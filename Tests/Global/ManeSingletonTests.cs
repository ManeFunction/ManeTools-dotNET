using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class ManeSingletonTests
    {
        [SetUp]
        public void SetUp()
        {
            AutoSingleton.ClearInstance();
            ManualSingleton.ClearInstance();
            NoCtorSingleton.ClearInstance();
        }

        [TearDown]
        public void TearDown()
        {
            AutoSingleton.ClearInstance();
            ManualSingleton.ClearInstance();
            NoCtorSingleton.ClearInstance();
        }

        [Test]
        public void Instance_CreatesViaPrivateConstructor()
        {
            Assert.IsFalse(AutoSingleton.IsReady());
            AutoSingleton instance = AutoSingleton.Instance;
            Assert.IsNotNull(instance);
            Assert.IsTrue(AutoSingleton.IsReady());
            Assert.AreSame(instance, AutoSingleton.Instance);
        }

        [Test]
        public void SetInstance_ReplacesCurrent()
        {
            ManualSingleton created = new();
            ManualSingleton.ClearInstance();
            ManualSingleton replacement = new();
            ManualSingleton.SetInstance(replacement);
            Assert.AreSame(replacement, ManualSingleton.Instance);
            Assert.AreNotSame(created, ManualSingleton.Instance);
        }

        [Test]
        public void SetInstance_Null_Throws() =>
            Assert.Throws<ArgumentNullException>(() => ManualSingleton.SetInstance(null));

        [Test]
        public void SecondConstructorCall_Throws()
        {
            _ = new ManualSingleton();
            Assert.Throws<InvalidOperationException>(() => _ = new ManualSingleton());
        }

        [Test]
        public void Instance_WithoutParameterlessConstructor_Throws() =>
            Assert.Throws<InvalidOperationException>(() => _ = NoCtorSingleton.Instance);

        [Test]
        public void ClearInstance_MakesIsReadyFalse()
        {
            _ = AutoSingleton.Instance;
            AutoSingleton.ClearInstance();
            Assert.IsFalse(AutoSingleton.IsReady());
        }

        private sealed class AutoSingleton : ManeSingleton<AutoSingleton>
        {
            private AutoSingleton() { }
        }

        private sealed class ManualSingleton : ManeSingleton<ManualSingleton>
        {
            public ManualSingleton() { }
        }

        private sealed class NoCtorSingleton : ManeSingleton<NoCtorSingleton>
        {
            public NoCtorSingleton(int unused) { }
        }
    }
}
