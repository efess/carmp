using CarMP.Reactive.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace CarMP.Tests
{
    
    
    /// <summary>
    ///This is a test class for MessageDispatcherTest and is intended
    ///to contain all MessageDispatcherTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MessageDispatcherTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for AddMessageObserver
        ///</summary>
        [TestMethod()]
        public void AddMessageObserverTest()
        {

            MessageDispatcher dispatcher = new MessageDispatcher(); // TODO: Initialize to an appropriate value
            IMessageObserver observer = new MessageObserverStub(string.Empty);
            dispatcher.AddMessageObserver(observer);
            var observerList = MessageDispatcher_Accessor.AttachShadow(dispatcher)._observerList;

            Assert.AreEqual(observerList.Count, 1);
        }

        /// <summary>
        ///A test for SendMessage
        ///</summary>
        [TestMethod()]
        public void SendMessageWithNameEmptyTest()
        {
            MessageDispatcher target = new MessageDispatcher(); // TODO: Initialize to an appropriate value
            Message pMessage = new Message(new string[]{}, MessageType.Trigger, null);

            var observer = new MessageObserverStub("foo");

            target.AddMessageObserver(observer);

            Assert.IsNull(observer.LastMessage);
            target.SendMessage(pMessage);
            Assert.AreSame(pMessage, observer.LastMessage);
        }

        /// <summary>
        ///A test for SendMessage
        ///</summary>
        [TestMethod()]
        public void SendMessageWithSpecificNameTest()
        {
            MessageDispatcher target = new MessageDispatcher(); // TODO: Initialize to an appropriate value
            Message pMessage = new Message(new string[] { "baz","foo" }, MessageType.Trigger, null);

            bool test = false;
            var observer = new MessageObserverStub("foo");

            target.AddMessageObserver(observer);

            Assert.IsNull(observer.LastMessage);
            target.SendMessage(pMessage);
            Assert.AreSame(pMessage, observer.LastMessage);
        }

        /// <summary>
        ///A test for SendMessage
        ///</summary>
        [TestMethod()]
        public void SendMessageWithSpecificNameNotMatchingObserverTest()
        {
            MessageDispatcher target = new MessageDispatcher(); // TODO: Initialize to an appropriate value
            Message pMessage = new Message(new string[] { "baz","foo" }, MessageType.Trigger, null);

            bool test = false;
            var observer = new MessageObserverStub("bar");

            target.AddMessageObserver(observer);

            Assert.IsNull(observer.LastMessage);
            target.SendMessage(pMessage);
            Assert.IsNull(observer.LastMessage);
        }

        /// <summary>
        ///A test for Subscribe
        ///</summary>
        [TestMethod()]
        public void SubscribeTest()
        {
            MessageDispatcher target = new MessageDispatcher(); // TODO: Initialize to an appropriate value
            IDisposable actual;
            try
            {
                actual = target.Subscribe(null);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(NotSupportedException));
            }

        }

        private class MessageObserverStub : IMessageObserver
        {
            public Message LastMessage { get; private set; }
            public string Name { get; private set; }

            public MessageObserverStub(string pName)
            {
                Name = pName;
            }

            public void ProcessMessage(Message pMessage)
            {
                LastMessage = pMessage;
            }
            public IDisposable DisposeUnsubscriber { get; set; }
        }
    }
}
