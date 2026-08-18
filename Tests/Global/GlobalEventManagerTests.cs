using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class GlobalEventManagerTests
    {
        [SetUp]
        public void SetUp() => GlobalEventManager.ClearInstance();

        [TearDown]
        public void TearDown() => GlobalEventManager.ClearInstance();

        [Test]
        public void RaiseEvent_InvokesMatchingListener()
        {
            int calls = 0;
            TestEvent received = null;
            GlobalEventManager.Instance.AddListener<TestEvent>(e =>
            {
                calls++;
                received = e;
            });

            TestEvent raised = new();
            GlobalEventManager.Instance.RaiseEvent(raised);
            Assert.AreEqual(1, calls);
            Assert.AreSame(raised, received);
        }

        [Test]
        public void RaiseEvent_DoesNotInvokeBaseTypeListeners()
        {
            int baseCalls = 0;
            int derivedCalls = 0;
            GlobalEventManager.Instance.AddListener<BaseEvent>(_ => baseCalls++);
            GlobalEventManager.Instance.AddListener<TestEvent>(_ => derivedCalls++);
            GlobalEventManager.Instance.RaiseEvent(new TestEvent());
            Assert.AreEqual(0, baseCalls);
            Assert.AreEqual(1, derivedCalls);
        }

        [Test]
        public void AddListener_SameDelegateTwice_IsIgnored()
        {
            int calls = 0;
            EventDelegate<TestEvent> listener = _ => calls++;
            GlobalEventManager.Instance.AddListener(listener);
            GlobalEventManager.Instance.AddListener(listener);
            GlobalEventManager.Instance.RaiseEvent(new TestEvent());
            Assert.AreEqual(1, calls);
        }

        [Test]
        public void RemoveListener_StopsInvocation()
        {
            int calls = 0;
            EventDelegate<TestEvent> listener = _ => calls++;
            GlobalEventManager.Instance.AddListener(listener);
            GlobalEventManager.Instance.RemoveListener(listener);
            GlobalEventManager.Instance.RaiseEvent(new TestEvent());
            Assert.AreEqual(0, calls);
        }

        [Test]
        public void RemoveListener_Null_DoesNotThrow() =>
            GlobalEventManager.Instance.RemoveListener<TestEvent>(null);

        [Test]
        public void RaiseEvent_WithNoListeners_DoesNotThrow() =>
            GlobalEventManager.Instance.RaiseEvent(new TestEvent());

        [Test]
        public void SenderEvent_ExposesSender()
        {
            object sender = new();
            UntypedSenderEvent untyped = new(sender);
            Assert.AreSame(sender, untyped.Sender);

            TypedSenderEvent typed = new("src");
            Assert.AreEqual("src", typed.Sender);
            Assert.AreEqual("src", ((SenderEvent)typed).Sender);
        }

        [Test]
        public void AddListener_AndRaiseEvent_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => GlobalEventManager.Instance.AddListener<TestEvent>(null));
            Assert.Throws<ArgumentNullException>(() => GlobalEventManager.Instance.RaiseEvent<TestEvent>(null));
        }

        private sealed class TestEvent : BaseEvent { }

        private sealed class UntypedSenderEvent : SenderEvent
        {
            public UntypedSenderEvent(object sender) : base(sender) { }
        }

        private sealed class TypedSenderEvent : SenderEvent<string>
        {
            public TypedSenderEvent(string sender) : base(sender) { }
        }
    }
}
