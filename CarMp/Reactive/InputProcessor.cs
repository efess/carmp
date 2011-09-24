using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Reactive.KeyInput;
using CarMP.Graphics.Geometry;
using CarMP.Reactive.Touch;
using CarMP.Graphics;
using CarMP.Graphics.Interfaces;

namespace CarMP.Reactive
{
    public class InputProcessor
    {
        //**** Mouse State Vars

        private bool _isDownHolding;
        private bool _isDown;
        private bool _isInSwipe;
        private Point _downPoint;
        private DateTime _downTime;
        private Point _previousPoint = new Point();
        private Point _startHighVelocityPoint;
        private DateTime _previousPointTime;
        private System.Threading.Timer _waitTimer = new System.Threading.Timer((o) => { });

        private VelocityAggregator _velocityAgg;

        //**** Feel Control Properties / Parameters

        public int TouchDownClickThreshold { get; set; }
        public int TouchDownClickDistanceTolerance { get; set; }
        public int TouchSwipeDistanceThreshold { get; set; }
        public int TouchSwipeVelocityThreshold { get; set; }
        public int TouchDownHoldThreshold { get; set; }

        public readonly Observables ObservableActions;

        public InputProcessor(IWindow pEventProducingWindow)
        {
            // Defaults
            TouchDownHoldThreshold = 1000;
            TouchDownClickThreshold = 200;
            TouchDownClickDistanceTolerance = 5;
            TouchSwipeDistanceThreshold = 50;
            TouchSwipeVelocityThreshold = 1000;

            ObservableActions = new Observables();

            _velocityAgg = new VelocityAggregator(5);

            // SetEvents
            pEventProducingWindow.SetProcessKeyPress(ProcessKeyPress);
            pEventProducingWindow.SetProcessMouseDown(ProcessMouseDown);
            pEventProducingWindow.SetProcessMouseMove(ProcessMouseMove);
            pEventProducingWindow.SetProcessMouseUp(ProcessMouseUp);
        }

        protected void ProcessKeyPress(char pChar, Keys pKeyChar)
        {
            SendKeyInput(new Key(pChar, pKeyChar));
        }

        protected void ProcessMouseUp(Point pMouseCoordinate)
        {
            // Check for Click
            _isDown = false;

            _isDownHolding = false;
            StopWaitTimer();

            if (TouchDownClickThreshold > (DateTime.Now - _downTime).TotalMilliseconds
                && Math.Abs(_downPoint.Y - pMouseCoordinate.Y) < TouchDownClickDistanceTolerance)
            {
                SendTouchGesture(new TouchGesture(GestureType.Click, pMouseCoordinate));
                return;
            }
        }

        protected void ProcessMouseDown(Point pMouseCoordinate)
        {
            _isInSwipe = false;
            _isDown = true;
            _isDownHolding = true;
            _downPoint = pMouseCoordinate;
            _downTime = DateTime.Now;
            _waitTimer = new System.Threading.Timer((o) =>
            {
                SendTouchGesture(new TouchGesture(GestureType.Hold, pMouseCoordinate));
                _isDownHolding = false;
            },
                null,
                TouchDownHoldThreshold,
                System.Threading.Timeout.Infinite);
        }

        protected void ProcessMouseMove(Point pMouseCoordinate)
        {
            DateTime dt = DateTime.Now;
            Point mousePoint = pMouseCoordinate;

            if (dt == _previousPointTime) return;

            bool mouseDown = _isDown;
            float seconds = (float)(dt - _previousPointTime).TotalSeconds;

            float xVelocity = Math.Abs((mousePoint.X - _previousPoint.X)
                / seconds);

            float yVelocity = Math.Abs((mousePoint.Y - _previousPoint.Y)
                / seconds);

            float directionalVelocity = LinearMath.DistanceBetweenTwoPoint(mousePoint, _previousPoint)
                / seconds;

            _velocityAgg.VelocityNow = new Velocity(xVelocity, yVelocity, directionalVelocity);
            Velocity velocityNow = _velocityAgg.GetVelocity;

            //System.Diagnostics.Debug.WriteLine("MouseMove- velocity: " + velocityNow + "  now: " + e.X.ToString() + "," + e.Y.ToString() + " previous: " + _previousPoint.X.ToString() + "," + _previousPoint.Y.ToString() + " " + e.Button.ToString());
            if (_isDownHolding
                && (Math.Abs(pMouseCoordinate.X - _downPoint.X) > 3
                || Math.Abs(pMouseCoordinate.Y - _downPoint.Y) > 3))
            {
                _isDownHolding = false;
                StopWaitTimer();
            }

            SendTouchMove(new TouchMove(mousePoint, mouseDown, velocityNow));

            if (mouseDown)
            {
                // Check for swipe
                if (!_isInSwipe && velocityNow.VelocityD > TouchSwipeVelocityThreshold)
                {
                    _isInSwipe = true;
                    _startHighVelocityPoint = mousePoint;
                }
                else if (_isInSwipe && velocityNow.VelocityD < TouchSwipeVelocityThreshold)
                    _isInSwipe = false;
                else if (_isInSwipe)
                {


                    // TODO: Chewck for angle. 
                    // HACK: the following code.
                    float x = _startHighVelocityPoint.X - mousePoint.X;
                    float y = _startHighVelocityPoint.Y - mousePoint.Y;
                    //DebugHandler.DebugPrint("IS IN SWIPE X: " + x.ToString()  + " y: " + y.ToString());
                    if (Math.Abs(x) > Math.Abs(y))
                    {
                        if (Math.Abs(x) > TouchSwipeDistanceThreshold)
                        {

                            _isInSwipe = false;
                            SendTouchGesture(new TouchGesture(
                                x > 0
                                ? GestureType.SwipeLeft
                                : GestureType.SwipeRight
                                , mousePoint));
                        }
                    }
                    else
                    {
                        if (Math.Abs(y) > TouchSwipeDistanceThreshold)
                        {

                            _isInSwipe = false;
                            SendTouchGesture(new TouchGesture(
                                y > 0
                                ? GestureType.SwipeUp
                                : GestureType.SwipeDown
                                , mousePoint));
                        }
                    }
                }
            }

            _previousPointTime = dt;
            _previousPoint = mousePoint;
        }


        private void ProcessVelocity(object pStateObject)
        {

            ////////////////////////
            // WHEN MOOUSE STOPS, VERY ABRUPTLY, Must maintain a decent velocity somehow?
            //

        }

        protected void SendKeyInput(Key pKeyInput)
        {
            ObservableActions.ObsKeyInput.PushKeyInput(pKeyInput);
        }

        protected void SendTouchGesture(TouchGesture pTouchGesture)
        {
            ObservableActions.ObsTouchGesture.PushTouchGesture(pTouchGesture);
            //DebugHandler.DebugPrint("Gesture: " + pTouchGesture.Gesture.ToString() + " at " + pTouchGesture.X.ToString() + "," + pTouchGesture.Y.ToString());
        }

        protected void SendTouchMove(TouchMove pTouchMove)
        {
            ObservableActions.ObsTouchMove.PushTouchMove(pTouchMove);
            //DebugHandler.DebugPrint("Velocity: " + pTouchMove.Velocity.VelocityD.ToString() + ", down: " + pTouchMove.TouchDown.ToString() + " at " + pTouchMove.X.ToString() + "," + pTouchMove.Y.ToString());
        }

        protected void StopWaitTimer()
        {
            _waitTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

    }
}
