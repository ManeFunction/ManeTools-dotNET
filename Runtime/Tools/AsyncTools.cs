using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Mane.DotNet
{
    /// <summary>
    /// Helpers for polling, wrapping callbacks as tasks, and observing task completion.
    /// </summary>
    public static class AsyncTools
    {
        /// <summary>
        /// Polls <paramref name="condition"/> until it returns true.
        /// This is not an event: a condition that is true only between polls can be missed.
        /// The first check runs on the caller thread; later checks run on a thread-pool thread
        /// and must not touch Unity objects.
        /// Throws <see cref="TimeoutException"/> if the timeout elapses.
        /// </summary>
        /// <param name="condition">Predicate to poll. Must be thread-safe after the first await.</param>
        /// <param name="delayMilliseconds">Wait between polls. Must be at least 1.</param>
        /// <param name="timeoutMilliseconds">
        /// Wall-clock limit, or <see cref="Timeout.Infinite"/>. The wait can overshoot by one delay.
        /// </param>
        /// <param name="cancellationToken">Cancels the wait with <see cref="OperationCanceledException"/>.</param>
        public static async Task WaitForCondition(Func<bool> condition, int delayMilliseconds = 100,
            int timeoutMilliseconds = Timeout.Infinite, CancellationToken cancellationToken = default)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            if (delayMilliseconds < 1)
                throw new ArgumentOutOfRangeException(nameof(delayMilliseconds));
            if (timeoutMilliseconds < Timeout.Infinite) // Timeout.Infinite is -1
                throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));

            cancellationToken.ThrowIfCancellationRequested();

            Stopwatch stopwatch = timeoutMilliseconds == Timeout.Infinite ? null : Stopwatch.StartNew();

            while (!condition())
            {
                cancellationToken.ThrowIfCancellationRequested();

                int delay = delayMilliseconds;
                if (stopwatch != null)
                {
                    long remaining = timeoutMilliseconds - stopwatch.ElapsedMilliseconds;
                    if (remaining <= 0)
                        throw new TimeoutException($"Condition was not met within {timeoutMilliseconds} ms.");

                    if (remaining < delay)
                        delay = (int)remaining;
                }

                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Starts a callback-based operation and returns a task that completes when the callback is invoked.
        /// Token cancel reports as <see cref="OperationCanceledException"/>.
        /// </summary>
        public static Task CreateTask(Action<Action> targetMethod,
            CancellationToken cancellationToken = default)
        {
            if (targetMethod == null)
                throw new ArgumentNullException(nameof(targetMethod));

            CallbackWait<bool> wait = new CallbackWait<bool>(cancellationToken);
            try
            {
                targetMethod(() => wait.Complete(true, true));
            }
            catch (Exception ex)
            {
                wait.Fail(ex);
            }

            return wait.Task;
        }

        /// <summary>
        /// Starts a callback-based operation and returns a task that completes when the callback is invoked with a result.
        /// Token cancel reports as <see cref="OperationCanceledException"/>.
        /// </summary>
        public static Task<T> CreateTask<T>(Action<Action<T>> targetMethod,
            CancellationToken cancellationToken = default)
        {
            if (targetMethod == null)
                throw new ArgumentNullException(nameof(targetMethod));

            CallbackWait<T> wait = new CallbackWait<T>(cancellationToken);
            try
            {
                targetMethod(result => wait.Complete(true, result));
            }
            catch (Exception ex)
            {
                wait.Fail(ex);
            }

            return wait.Task;
        }

        /// <summary>
        /// Invokes <paramref name="callback"/> when the task ends. Does not wait.
        /// Runs on the current synchronization context when one exists (Unity main thread).
        /// <paramref name="cancellationToken"/> cancels this wait only, not the task.
        /// </summary>
        /// <param name="task">The task to observe.</param>
        /// <param name="cancellationToken">The cancellation token to cancel this wait only.</param>
        /// <param name="callback">
        /// <see langword="null"/> exception on success; otherwise the failure or wait-cancel error.
        /// </param>
        public static void OnComplete(this Task task, Action<Exception> callback,
            CancellationToken cancellationToken = default)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            Continue(task, cancellationToken,
                _ => callback(null),
                callback);
        }

        /// <summary>
        /// Invokes <paramref name="callback"/> when the task ends. Does not wait.
        /// Runs on the current synchronization context when one exists (Unity main thread).
        /// <paramref name="cancellationToken"/> cancels this wait only, not the task.
        /// </summary>
        /// <param name="task">The task to observe.</param>
        /// <param name="cancellationToken">The cancellation token to cancel this wait only.</param>
        /// <param name="callback">
        /// Result and <see langword="null"/> exception on success; default result and the error otherwise.
        /// </param>
        public static void OnComplete<T>(this Task<T> task, Action<T, Exception> callback,
            CancellationToken cancellationToken = default)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            Continue(task, cancellationToken,
                t => callback(((Task<T>)t).Result, null),
                error => callback(default, error));
        }

        private sealed class CallbackWait<T>
        {
            private readonly TaskCompletionSource<T> _tcs;
            private CancellationTokenRegistration _registration;

            public CallbackWait(CancellationToken cancellationToken)
            {
                _tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
                if (cancellationToken.CanBeCanceled)
                    _registration = cancellationToken.Register(() => _tcs.TrySetCanceled(cancellationToken), false);
            }

            public Task<T> Task => _tcs.Task;

            public void Complete(bool success, T result)
            {
                _registration.Dispose();
                if (success)
                    _tcs.TrySetResult(result);
                else
                    _tcs.TrySetException(new InvalidOperationException("Callback reported failure."));
            }

            public void Fail(Exception exception)
            {
                _registration.Dispose();
                _tcs.TrySetException(exception);
            }
        }

        private static void Continue(Task task, CancellationToken cancellationToken,
            Action<Task> onSuccess, Action<Exception> onUnsuccessful)
        {
            Task continuation = task.ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        onSuccess(t);
                        return;
                    }

                    onUnsuccessful(ErrorFrom(t));
                },
                cancellationToken,
                TaskContinuationOptions.ExecuteSynchronously,
                CurrentScheduler());

            continuation.ContinueWith(
                canceled =>
                {
                    onUnsuccessful(new OperationCanceledException(cancellationToken));
                    task.ContinueWith(
                        t =>
                        {
                            _ = t.Exception;
                        },
                        TaskContinuationOptions.OnlyOnFaulted);
                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.ExecuteSynchronously,
                CurrentScheduler());
        }

        private static Exception ErrorFrom(Task task)
        {
            if (task.IsFaulted)
                return task.Exception?.GetBaseException() ?? task.Exception;
            if (task.IsCanceled)
                return new OperationCanceledException();
            return new InvalidOperationException($"Task ended in unexpected status {task.Status}.");
        }

        private static TaskScheduler CurrentScheduler() =>
            SynchronizationContext.Current != null
                ? TaskScheduler.FromCurrentSynchronizationContext()
                : TaskScheduler.Default;
    }
}
