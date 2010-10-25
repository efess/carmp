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
            var observer = new IMessageObserver_Impl(null);

            MessageDispatcher dispatcher = new MessageDispatcher(); // TODO: Initialize to an appropriate value
            IMessageObserver pObserver = null; // TODO: Initialize to an appropriate value
            dispatcher.AddMessageObserver(pObserver);
            var observerList = MessageDispatcher_Accessor.AttachShadow(dispatcher)._observerList;

            Assert.AreEqual(observerList.Count, 1);
            Assert.AreEqual(observerList[0], observer);
        }

        /// <summary>
        ///A test for SendMessage
        ///</summary>
        [TestMethod()]
        public void SendMessageWithNameEmptyTest()
        {
            MessageDispatcher target = new MessageDispatcher(); // TODO: Initialize to an appropriate value
            Message pMessage = null; // TODO: Initialize to an appropriate value

            bool test = false;
            var observer = new Mock<IMessageObserver_Impl>();
            observer.Setup(c => c.ProcessMessage(It.IsAny<Message>()))
                .Callback(() => test = true);
            
            target.AddMessageObserver(observer.Object as IMessageObserver);
            
            Assert.IsFalse(test);
            target.SendMessage(pMessage); 
            Assert.IsTrue(test);
        }

        /// <summary>
        ///A test for SendMessage
        ///</summary>
        [TestMethod()]
        public void SendMessageWithSpecificNameTest()
        {
            MessageDispatcher target = new MessageDispatcher(); // TODO: Initialize to an appropriate value
            Message pMessage = null; // TODO: Initialize to an appropriate value

            bool test = false;
            var observer = new Mock<IMessageObserver_Impl>();
            observer.Setup(c => c.ProcessMessage(It.IsAny<Message>()))
                .Callback(() => test = true);

            target.SendMessage(pMessage);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
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
    }
}
