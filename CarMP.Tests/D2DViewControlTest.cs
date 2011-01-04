using CarMP.ViewControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMP.Reactive.KeyInput;
using CarMP.Reactive.Touch;
using CarMP.Direct2D;
using CarMP.Reactive;

namespace CarMP.Tests
{
    
    
    /// <summary>
    ///This is a test class for D2DViewControlTest and is intended
    ///to contain all D2DViewControlTest Unit Tests
    ///</summary>
    [TestClass()]
    public class D2DViewControlTest
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

        internal class D2DViewControlStub : D2DViewControl
        {
            protected override void OnRender(RenderTargetWrapper pRenderTarget)
            {
                throw new NotImplementedException();
            }
        }

        internal virtual D2DViewControl CreateD2DViewControl()
        {
            // TODO: Instantiate an appropriate concrete class.
            return new D2DViewControlStub();
        }

        /// <summary>
        ///A test for AddViewControl
        ///</summary>
        [TestMethod()]
        public void AddViewControlTest()
        {
            D2DViewControl target = CreateD2DViewControl();
            var add = CreateD2DViewControl();
            target.AddViewControl(add);
            Assert.AreSame(add, target.ViewControls[0]);
        }

        /// <summary>
        ///A test for Clear
        ///</summary>
        [TestMethod()]
        public void ClearTest()
        {
            D2DViewControl target = CreateD2DViewControl(); 
            target.AddViewControl(CreateD2DViewControl());
            target.AddViewControl(CreateD2DViewControl());
            target.Clear();

            Assert.AreEqual(0, target.ViewControls.Count);
        }

        /// <summary>
        ///A test for ConvertScreenToControlPoint
        ///</summary>
        [TestMethod()]
        public void ConvertScreenToControlPointTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            Point2F pPointToConvert = new Point2F(); // TODO: Initialize to an appropriate value
            Point2F expected = new Point2F(); // TODO: Initialize to an appropriate value
            Point2F actual;
            actual = target.ConvertScreenToControlPoint(pPointToConvert);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetAllowedRenderingArea
        ///</summary>
        [TestMethod()]
        public void GetAllowedRenderingAreaTest()
        {
            D2DViewControl root = CreateD2DViewControl();
            root.Bounds = new RectF(0, 0, 5, 5);
             
            D2DViewControl immediateParent = CreateD2DViewControl();
            immediateParent.Bounds = new RectF(2, -2, 7, 3);
            root.AddViewControl(immediateParent);

            D2DViewControl child = CreateD2DViewControl();
            child.Bounds = new RectF(-4, 0, 1, 5);
            immediateParent.AddViewControl(child);

            RectF actual = child.GetAllowedScreenRenderingArea();
            RectF expected = new RectF(4, 2, 5, 5);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetScreenBounds
        ///</summary>
        [TestMethod()]
        public void GetScreenBoundsTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            RectF expected = new RectF(); // TODO: Initialize to an appropriate value
            RectF actual;
            actual = target.GetScreenBounds();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetScreenPoint
        ///</summary>
        [TestMethod()]
        public void GetScreenPointTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            Point2F expected = new Point2F(); // TODO: Initialize to an appropriate value
            Point2F actual;
            actual = target.GetScreenPoint();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetScreenPoint
        ///</summary>
        [TestMethod()]
        public void GetScreenPointTest1()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            Point2F pPointToAdd = new Point2F(); // TODO: Initialize to an appropriate value
            Point2F expected = new Point2F(); // TODO: Initialize to an appropriate value
            Point2F actual;
            actual = target.GetScreenPoint(pPointToAdd);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetViewControlContainingPoint
        ///</summary>
        [TestMethod()]
        public void GetViewControlContainingPointTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            D2DViewControl pViewControl = null; // TODO: Initialize to an appropriate value
            Point2F pPoint = new Point2F(); // TODO: Initialize to an appropriate value
            D2DViewControl expected = null; // TODO: Initialize to an appropriate value
            D2DViewControl actual;
            actual = target.GetViewControlContainingPoint(pViewControl, pPoint);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetViewControlContainingPoint
        ///</summary>
        [TestMethod()]
        public void GetViewControlContainingPointTest1()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            Point2F pPoint = new Point2F(); // TODO: Initialize to an appropriate value
            D2DViewControl expected = null; // TODO: Initialize to an appropriate value
            D2DViewControl actual;
            actual = target.GetViewControlContainingPoint(pPoint);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IndexOf
        ///</summary>
        [TestMethod()]
        public void IndexOfTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            D2DViewControl pViewControl = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.IndexOf(pViewControl);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        internal virtual D2DViewControl_Accessor CreateD2DViewControl_Accessor()
        {
            // TODO: Instantiate an appropriate concrete class.
            D2DViewControl_Accessor target = null;
            return target;
        }

        /// <summary>
        ///A test for OnInputLeave
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CarMP.exe")]
        public void OnInputLeaveTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            D2DViewControl_Accessor target = new D2DViewControl_Accessor(param0); // TODO: Initialize to an appropriate value
            target.OnInputLeave();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnKeyPressed
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CarMP.exe")]
        public void OnKeyPressedTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            D2DViewControl_Accessor target = new D2DViewControl_Accessor(param0); // TODO: Initialize to an appropriate value
            Key pKey = null; // TODO: Initialize to an appropriate value
            target.OnKeyPressed(pKey);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnRender
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CarMP.exe")]
        public void OnRenderTest()
        {
            // Private Accessor for OnRender is not found. Please rebuild the containing project or run the Publicize.exe manually.
            Assert.Inconclusive("Private Accessor for OnRender is not found. Please rebuild the containing project" +
                    " or run the Publicize.exe manually.");
        }

        /// <summary>
        ///A test for OnRenderStart
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CarMP.exe")]
        public void OnRenderStartTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            D2DViewControl_Accessor target = new D2DViewControl_Accessor(param0); // TODO: Initialize to an appropriate value
            target.OnRenderStart();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnRenderStop
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CarMP.exe")]
        public void OnRenderStopTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            D2DViewControl_Accessor target = new D2DViewControl_Accessor(param0); // TODO: Initialize to an appropriate value
            target.OnRenderStop();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnSizeChanged
        ///</summary>
        [TestMethod()]
        public void OnSizeChangedTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            EventArgs e = null; // TODO: Initialize to an appropriate value
            target.OnSizeChanged(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnTouchGesture
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CarMP.exe")]
        public void OnTouchGestureTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            D2DViewControl_Accessor target = new D2DViewControl_Accessor(param0); // TODO: Initialize to an appropriate value
            TouchGesture pTouchGesture = null; // TODO: Initialize to an appropriate value
            target.OnTouchGesture(pTouchGesture);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnTouchMove
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CarMP.exe")]
        public void OnTouchMoveTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            D2DViewControl_Accessor target = new D2DViewControl_Accessor(param0); // TODO: Initialize to an appropriate value
            TouchMove pTouchMove = null; // TODO: Initialize to an appropriate value
            target.OnTouchMove(pTouchMove);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PreRender
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CarMP.exe")]
        public void PreRenderTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            D2DViewControl_Accessor target = new D2DViewControl_Accessor(param0); // TODO: Initialize to an appropriate value
            target.PreRender();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Remove
        ///</summary>
        [TestMethod()]
        public void RemoveTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            D2DViewControl pViewControl = null; // TODO: Initialize to an appropriate value
            target.Remove(pViewControl);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Render
        ///</summary>
        [TestMethod()]
        public void RenderTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            RenderTargetWrapper pRenderTarget = null; // TODO: Initialize to an appropriate value
            target.Render(pRenderTarget);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SendUpdate
        ///</summary>
        [TestMethod()]
        public void SendUpdateTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            ReactiveUpdate pReactiveUpdate = null; // TODO: Initialize to an appropriate value
            target.SendUpdate(pReactiveUpdate);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetInputControl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CarMP.exe")]
        public void SetInputControlTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            D2DViewControl_Accessor target = new D2DViewControl_Accessor(param0); // TODO: Initialize to an appropriate value
            target.SetInputControl();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for StartRender
        ///</summary>
        [TestMethod()]
        public void StartRenderTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            target.StartRender();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for StopRenderer
        ///</summary>
        [TestMethod()]
        public void StopRendererTest()
        {
            D2DViewControl target = CreateD2DViewControl(); // TODO: Initialize to an appropriate value
            target.StopRenderer();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
