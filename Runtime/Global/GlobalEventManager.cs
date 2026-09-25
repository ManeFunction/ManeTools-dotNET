using System;
using System.Collections.Generic;

namespace Mane.DotNet
{
    /// <summary>
    /// In-process event bus keyed by event type.
    /// Raising an event also invokes listeners registered for its base types.
    /// Matching listeners run in subscription order.
    /// </summary>
    public class GlobalEventManager : ManeSingleton<GlobalEventManager>
    {
        private readonly List<Listener> _listeners = new();
        private readonly object _sync = new();

        private GlobalEventManager() { }

        /// <summary>
        /// Add a listener to an event
        /// </summary>
        /// <param name="listener">The listener to add</param>
        /// <typeparam name="T">Type of event, derived from <see cref="BaseEvent"/></typeparam>
        public void AddListener<T>(EventDelegate<T> listener) where T : BaseEvent
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            Type eventType = typeof(T);
            lock (_sync)
            {
                if (Contains(eventType, listener))
                    return;

                _listeners.Add(new Listener(eventType, listener, e => listener((T)e)));
            }
        }

        /// <summary>
        /// Remove a listener from an event
        /// </summary>
        /// <param name="listener">The listener to remove</param>
        /// <typeparam name="T">Type of event, derived from <see cref="BaseEvent"/></typeparam>
        public void RemoveListener<T>(EventDelegate<T> listener) where T : BaseEvent
        {
            if (listener == null)
                return;

            Type eventType = typeof(T);
            lock (_sync)
            {
                for (int i = 0; i < _listeners.Count; i++)
                {
                    if (!_listeners[i].Matches(eventType, listener))
                        continue;

                    _listeners.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Raise an event. Every listener whose type is <paramref name="e"/> or a base of it runs,
        /// in subscription order. A derived instance still notifies its own listeners when the
        /// call is typed as a base.
        /// </summary>
        /// <param name="e">The event to raise</param>
        /// <typeparam name="T">Type of event, derived from <see cref="BaseEvent"/></typeparam>
        public void RaiseEvent<T>(T e) where T : BaseEvent
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            Type runtimeType = e.GetType();
            List<Action<BaseEvent>> callbacks = new();
            lock (_sync)
            {
                foreach (var listener in _listeners)
                {
                    if (listener.Accepts(runtimeType))
                        callbacks.Add(listener.Invoke);
                }
            }

            foreach (var callback in callbacks)
                callback(e);
        }

        private bool Contains(Type eventType, Delegate listener)
        {
            foreach (var eventTypeListener in _listeners)
            {
                if (eventTypeListener.Matches(eventType, listener))
                    return true;
            }

            return false;
        }

        private sealed class Listener
        {
            private readonly Type _eventType;
            private readonly Delegate _original;

            public Action<BaseEvent> Invoke { get; }

            public Listener(Type eventType, Delegate original, Action<BaseEvent> invoke)
            {
                _eventType = eventType;
                _original = original;
                Invoke = invoke;
            }

            public bool Accepts(Type runtimeType) => _eventType.IsAssignableFrom(runtimeType);

            public bool Matches(Type eventType, Delegate listener) =>
                _eventType == eventType && _original.Equals(listener);
        }
    }
    
    /// <summary>
    /// Delegate for global events
    /// </summary>
    /// <typeparam name="T">Type of event, derived from <see cref="BaseEvent"/></typeparam>
    public delegate void EventDelegate<in T>(T e) where T : BaseEvent;

    /// <summary>
    /// Base class for all events
    /// </summary>
    public abstract class BaseEvent { }

    /// <summary>
    /// Base class for events with an untyped sender.
    /// </summary>
    public abstract class SenderEvent : BaseEvent
    {
        /// <summary>
        /// The object that sent this event
        /// </summary>
        public object Sender { get; }
        
        /// <summary>
        /// Creates an event with an untyped sender.
        /// </summary>
        /// <param name="sender">Object that raised the event. May be null.</param>
        protected SenderEvent(object sender) => Sender = sender;
    }
    
    /// <summary>
    /// Base class for all events with a typed sender
    /// </summary>
    /// <typeparam name="T">Type of sender</typeparam>
    public abstract class SenderEvent<T> : SenderEvent where T : class
    {
        /// <summary>
        /// The object that sent this event
        /// </summary>
        public new T Sender => (T)base.Sender;

        /// <summary>
        /// Creates an event with a typed sender.
        /// </summary>
        /// <param name="sender">Object that raised the event. May be null.</param>
        protected SenderEvent(T sender) : base(sender) { }
    }
}