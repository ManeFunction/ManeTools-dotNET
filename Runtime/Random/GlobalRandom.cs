using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Process-wide <see cref="IRandom"/> slot.
    /// Same lifecycle as <see cref="ManeSingleton{T}"/>: <see cref="Instance"/>, <see cref="IsReady"/>,
    /// <see cref="SetInstance"/>, <see cref="ClearInstance"/>.
    /// The first access to <see cref="Instance"/> stores a <see cref="ManeRandom"/>.
    /// Calls through <see cref="Instance"/> are serialized, including when the stored source is not.
    /// </summary>
    public static class GlobalRandom
    {
        private static readonly object _lock = new();
        private static readonly Gate _gate = new();
        private static volatile IRandom _source;

        /// <summary>
        /// The current source. Creates a <see cref="ManeRandom"/> if none has been set.
        /// The same gate is returned every time. Overlapping draws on it are serialized.
        /// </summary>
        public static IRandom Instance
        {
            get
            {
                EnsureSource();
                return _gate;
            }
        }

        /// <summary>
        /// Returns true if a source is already stored, without creating one.
        /// </summary>
        public static bool IsReady() => _source != null;

        /// <summary>
        /// Replaces the stored source. Null throws.
        /// </summary>
        public static void SetInstance(IRandom instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            lock (_lock)
                _source = instance;
        }

        /// <summary>
        /// Clears the stored source.
        /// </summary>
        public static void ClearInstance()
        {
            lock (_lock)
                _source = null;
        }

        private static void EnsureSource()
        {
            if (_source != null)
                return;

            lock (_lock)
            {
                if (_source == null)
                    _source = new ManeRandom();
            }
        }

        private sealed class Gate : IRandom
        {
            public int Seed
            {
                get
                {
                    lock (_lock)
                        return Source.Seed;
                }
            }

            public int Next(int min, int max)
            {
                lock (_lock)
                    return Source.Next(min, max);
            }

            public double Range01Double()
            {
                lock (_lock)
                    return Source.Range01Double();
            }

            public float Range01()
            {
                lock (_lock)
                    return Source.Range01();
            }

            private static IRandom Source
            {
                get
                {
                    if (_source == null)
                        _source = new ManeRandom();

                    return _source;
                }
            }
        }
    }
}
