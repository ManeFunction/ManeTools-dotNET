using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Thread-safe singleton base. The first access to <see cref="Instance"/> creates
    /// the type via its parameterless constructor unless <see cref="SetInstance"/> was called first.
    /// </summary>
    /// <typeparam name="T">Concrete singleton type.</typeparam>
    public abstract class ManeSingleton<T> where T : ManeSingleton<T>
    {
        private static readonly object _lock = new();
        private static volatile T _instance;

        /// <summary>
        /// The current singleton instance. Creates one if none has been set.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_lock)
                {
                    if (_instance == null)
                        CreateInstance();

                    return _instance;
                }
            }
        }

        /// <summary>
        /// Returns true if an instance already exists, without creating one.
        /// </summary>
        public static bool IsReady() => _instance != null;

        /// <summary>
        /// Replaces the current instance. Use this when the type has no parameterless constructor.
        /// </summary>
        /// <param name="instance">The instance to store. Must not be null.</param>
        public static void SetInstance(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            lock (_lock)
                _instance = instance;
        }

        /// <summary>
        /// Clears the stored instance.
        /// </summary>
        public static void ClearInstance()
        {
            lock (_lock)
                _instance = null;
        }

        /// <summary>
        /// Registers this object as the singleton instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when another instance is already registered.
        /// </exception>
        protected ManeSingleton()
        {
            lock (_lock)
            {
                if (_instance != null && !ReferenceEquals(_instance, this))
                    throw new InvalidOperationException(
                        $"{typeof(T).Name} singleton is already initialized.");

                _instance = (T)this;
            }
        }

        private static void CreateInstance()
        {
            try
            {
                Activator.CreateInstance(typeof(T), true);
            }
            catch (MissingMethodException)
            {
                throw new InvalidOperationException(
                    $"Failed to create {typeof(T).Name} singleton. " +
                    "Provide a parameterless constructor or call SetInstance.");
            }
        }
    }
}
