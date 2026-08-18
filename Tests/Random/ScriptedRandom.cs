using System;
using System.Collections.Generic;

namespace Mane.DotNet.Tests
{
    /// <summary>
    /// Deterministic <see cref="IRandom"/> for tests. Dequeues scripted <see cref="Next"/> results;
    /// when the queue is empty, returns <c>min</c>.
    /// </summary>
    internal sealed class ScriptedRandom : IRandom
    {
        private readonly Queue<int> _nextValues = new();

        public int Seed => 0;

        public ScriptedRandom(params int[] nextValues)
        {
            if (nextValues == null)
                return;

            foreach (int value in nextValues)
                _nextValues.Enqueue(value);
        }

        public int Next(int min, int max) =>
            _nextValues.Count > 0 ? _nextValues.Dequeue() : min;

        public double Range01Double() => 0d;

        public float Range01() => 0f;
    }
}
