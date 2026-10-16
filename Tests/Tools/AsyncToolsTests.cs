using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class AsyncToolsTests
    {
        private const int HangTimeoutMs = 2000;

        #region WaitForCondition

        [Test]
        public async Task WaitForCondition_AlreadyTrue_Completes() =>
            await AsyncTools.WaitForCondition(() => true, delayMilliseconds: 1);

        [Test]
        public void WaitForCondition_BecomesTrue()
        {
            int n = 0;
            RunOnThreadPool(() =>
                AsyncTools.WaitForCondition(
                    () => Interlocked.Increment(ref n) >= 2,
                    delayMilliseconds: 1,
                    timeoutMilliseconds: 1000));
            Assert.GreaterOrEqual(n, 2);
        }

        [Test]
        public void WaitForCondition_Timeout_Throws() =>
            AssertThrowsOnThreadPool<TimeoutException>(() =>
                AsyncTools.WaitForCondition(() => false, delayMilliseconds: 1, timeoutMilliseconds: 50));

        [Test]
        public void WaitForCondition_Canceled_Throws()
        {
            using CancellationTokenSource cts = new();
            cts.Cancel();
            AssertThrowsOnThreadPool<OperationCanceledException>(() =>
                AsyncTools.WaitForCondition(() => false, delayMilliseconds: 10, cancellationToken: cts.Token));
        }

        [Test]
        public void WaitForCondition_InvalidArgs_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => AsyncTools.WaitForCondition(null));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                AsyncTools.WaitForCondition(() => true, delayMilliseconds: -1));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                AsyncTools.WaitForCondition(() => true, delayMilliseconds: 0));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                AsyncTools.WaitForCondition(() => true, timeoutMilliseconds: -2));
        }

        #endregion

        #region CreateTask

        [Test]
        public async Task CreateTask_CompletesWhenInvoked()
        {
            await AsyncTools.CreateTask(done => done());
            Assert.AreEqual(7, await AsyncTools.CreateTask<int>(done => done(7)));
        }

        [Test]
        public void CreateTask_TargetThrows_FaultsTask()
        {
            Task task = AsyncTools.CreateTask(_ => throw new InvalidOperationException("x"));
            Assert.ThrowsAsync<InvalidOperationException>(() => task);
        }

        [Test]
        public void CreateTask_Cancel_Throws()
        {
            using CancellationTokenSource cts = new();
            Task task = AsyncTools.CreateTask(_ => { }, cts.Token);
            cts.Cancel();
            CompleteOrThrow<OperationCanceledException>(task);
        }

        [Test]
        public void CreateTask_AlreadyCanceled_DoesNotStart()
        {
            using CancellationTokenSource cts = new();
            cts.Cancel();
            bool started = false;
            bool startedT = false;

            Task task = AsyncTools.CreateTask(_ => started = true, cts.Token);
            Task<int> taskT = AsyncTools.CreateTask<int>(_ => startedT = true, cts.Token);

            Assert.IsFalse(started);
            Assert.IsFalse(startedT);
            Assert.IsTrue(task.IsCanceled);
            Assert.IsTrue(taskT.IsCanceled);
        }

        [Test]
        public void CreateTask_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AsyncTools.CreateTask(null));
            Assert.Throws<ArgumentNullException>(() => AsyncTools.CreateTask<int>(null));
        }

        #endregion

        #region OnComplete

        [Test]
        public void OnComplete_Success_PassesNullException()
        {
            Exception error = new InvalidOperationException("sentinel");
            RunOnThreadPool(() =>
            {
                Task.CompletedTask.OnComplete(e => error = e);
                return Task.CompletedTask;
            });
            Assert.IsNull(error);
        }

        [Test]
        public void OnComplete_Result_PassesValue()
        {
            Exception error = new InvalidOperationException("sentinel");
            int value = 0;
            RunOnThreadPool(() =>
            {
                Task.FromResult(42).OnComplete((v, e) =>
                {
                    value = v;
                    error = e;
                });
                return Task.CompletedTask;
            });
            Assert.IsNull(error);
            Assert.AreEqual(42, value);
        }

        [Test]
        public void OnComplete_Faulted_PassesException()
        {
            Exception error = null;
            Exception errorT = null;
            int value = -1;
            RunOnThreadPool(() =>
            {
                Task.FromException(new InvalidOperationException("x")).OnComplete(e => error = e);
                Task.FromException<int>(new InvalidOperationException("y")).OnComplete((v, e) =>
                {
                    value = v;
                    errorT = e;
                });
                return Task.CompletedTask;
            });
            Assert.IsInstanceOf<InvalidOperationException>(error);
            Assert.AreEqual("x", error.Message);
            Assert.IsInstanceOf<InvalidOperationException>(errorT);
            Assert.AreEqual("y", errorT.Message);
            Assert.AreEqual(0, value);
        }

        [Test]
        public void OnComplete_CanceledTask_PassesOperationCanceled()
        {
            Exception error = null;
            RunOnThreadPool(() =>
            {
                Task.FromCanceled(new CancellationToken(true)).OnComplete(e => error = e);
                return Task.CompletedTask;
            });
            Assert.IsInstanceOf<OperationCanceledException>(error);
        }

        [Test]
        public void OnComplete_CanceledWait_DoesNotCancelTask()
        {
            using CancellationTokenSource cts = new();
            TaskCompletionSource<int> source = new();
            Exception error = null;
            int value = -1;

            RunOnThreadPool(() =>
            {
                source.Task.OnComplete((v, e) =>
                {
                    value = v;
                    error = e;
                }, cts.Token);
                cts.Cancel();

                SpinWait.SpinUntil(() => error != null, HangTimeoutMs);
                return Task.CompletedTask;
            });

            Assert.IsInstanceOf<OperationCanceledException>(error);
            Assert.AreEqual(0, value);
            Assert.IsFalse(source.Task.IsCompleted);

            source.SetResult(7);
            Assert.AreEqual(7, source.Task.Result);
        }

        [Test]
        public void OnComplete_CallbackThrows_RaisesOnContext()
        {
            Exception raised = null;
            Exception raisedT = null;
            RunOnThreadPool(() =>
            {
                ManualContext context = new();
                SynchronizationContext previous = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(context);
                try
                {
                    Task.CompletedTask.OnComplete(_ => throw new InvalidOperationException("x"));
                    raised = context.RunUntilThrows(HangTimeoutMs);
                    Task.FromResult(1).OnComplete((_, __) => throw new InvalidOperationException("y"));
                    raisedT = context.RunUntilThrows(HangTimeoutMs);
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(previous);
                }

                return Task.CompletedTask;
            });

            Assert.IsInstanceOf<InvalidOperationException>(raised);
            Assert.AreEqual("x", raised.Message);
            Assert.IsInstanceOf<InvalidOperationException>(raisedT);
            Assert.AreEqual("y", raisedT.Message);
        }

        [Test]
        public void OnComplete_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((Task)null).OnComplete(_ => { }));
            Assert.Throws<ArgumentNullException>(() => Task.CompletedTask.OnComplete(null));
            Assert.Throws<ArgumentNullException>(() => ((Task<int>)null).OnComplete((_, __) => { }));
            Assert.Throws<ArgumentNullException>(() => Task.FromResult(1).OnComplete(null));
        }

        #endregion

        /// <summary>
        /// Edit Mode deadlocks if <see cref="Task.Delay"/> resumes on the Unity sync context
        /// while the test runner is blocking the main thread. Run those awaits on the thread pool
        /// and fail instead of hanging forever.
        /// </summary>
        private static void RunOnThreadPool(Func<Task> action, int timeoutMs = HangTimeoutMs)
        {
            CompleteOrThrow(Task.Run(action), timeoutMs);
        }

        private static void AssertThrowsOnThreadPool<T>(Func<Task> action, int timeoutMs = HangTimeoutMs)
            where T : Exception
        {
            CompleteOrThrow<T>(Task.Run(action), timeoutMs);
        }

        private static void CompleteOrThrow(Task task, int timeoutMs = HangTimeoutMs)
        {
            if (!WaitFinished(task, timeoutMs))
                Assert.Fail($"Async test hung for {timeoutMs} ms.");
            if (task.IsFaulted || task.IsCanceled)
                ExceptionDispatchInfo.Capture(Inner(task)).Throw();
        }

        private static void CompleteOrThrow<T>(Task task, int timeoutMs = HangTimeoutMs)
            where T : Exception
        {
            if (!WaitFinished(task, timeoutMs))
                Assert.Fail($"Async test hung for {timeoutMs} ms.");
            Assert.IsInstanceOf<T>(Inner(task));
        }

        private static bool WaitFinished(Task task, int timeoutMs)
        {
            try
            {
                return task.Wait(timeoutMs);
            }
            catch (AggregateException)
            {
                return true;
            }
        }

        /// <summary>
        /// Context that queues posted work until the test runs it, like Unity's main-thread context.
        /// </summary>
        private sealed class ManualContext : SynchronizationContext
        {
            private readonly BlockingCollection<(SendOrPostCallback, object)> _queue = new();

            public override void Post(SendOrPostCallback d, object state) => _queue.Add((d, state));

            // Runs posted work on this thread and returns the first exception it throws, or null on timeout.
            public Exception RunUntilThrows(int timeoutMs)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                while (stopwatch.ElapsedMilliseconds < timeoutMs)
                {
                    if (!_queue.TryTake(out (SendOrPostCallback callback, object state) item, 10))
                        continue;

                    try
                    {
                        item.callback(item.state);
                    }
                    catch (Exception exception)
                    {
                        return exception;
                    }
                }

                return null;
            }
        }

        private static Exception Inner(Task task)
        {
            if (task.Exception != null)
                return task.Exception.Flatten().InnerException ?? task.Exception;
            if (task.IsCanceled)
                return new TaskCanceledException(task);
            return new AssertionException("Task succeeded but an exception was expected.");
        }
    }
}
