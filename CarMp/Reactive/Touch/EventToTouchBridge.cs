using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CarMp.Reactive.Touch
{
    public class EventToTouchBridge
    {
        //**** Mouse State Vars

        private bool _isDown;
        private bool _isInSwipe;
        private PointF _downPoint;
        private DateTime _downTime;
        private PointF _previousPoint;
        private PointF _startHighVelocityPoint;
        private DateTime _previousPointTime;
        
        private int _moveVelocity;

        //****

        //**** Feel Control Properties / Parameters

        public int TouchDownClickThreshold { get; set; }
        public int TouchDownClickDistanceTolerance { get; set; }
        public int TouchSwipeDistanceThreshold { get; set; }
        public int TouchSwipeVelocityThreshold { get; set; }

        //****
        private Control _control;
        public readonly TouchObservables ObservablTouchActions;

        public EventToTouchBridge(Control pControl)
        {
            if (pControl == null)
                throw new ArgumentNullException("pControl");
            
            _control = pControl;

            // Defaults
            TouchDownClickThreshold = 200;
            TouchDownClickDistanceTolerance = 5;
            TouchSwipeDistanceThreshold = 200;
            TouchSwipeVelocityThreshold = 1000;

            CreateEventSubscriptions();
            ObservablTouchActions = new TouchObservables();
        }

        public void CreateEventSubscriptions()
        {
            _control.MouseUp += (sender, e) => ProcessMouseUp(e);
            _control.MouseDown += (sender, e) => ProcessMouseDown(e);
            _control.MouseMove += (sender, e) => ProcessMouseMove(e);
        }

        private void ProcessMouseUp(MouseEventArgs e)
        {
            _isDown = false;

            // Check for Click
            if (TouchDownClickThreshold > (DateTime.Now - _downTime).TotalMilliseconds
                && Math.Abs(_downPoint.Y - e.Y) < TouchDownClickDistanceTolerance)
            {
                SendTouchGesture(new TouchGesture(GestureType.Click, e.Location));
                return;
            }
        }

        private void ProcessMouseDown(MouseEventArgs e)
        {
            _isInSwipe = false;
            _isDown = true;
            _downPoint = e.Location;
            _downTime = DateTime.Now;
        }

        private void ProcessMouseMove(MouseEventArgs e)
        {
            DateTime dt = DateTime.Now;
            if (dt == _previousPointTime) return;

            float velocity = LinearMath.DistanceBetweenTwoPoint(e.Location, _previousPoint)
                / (float)((dt - _previousPointTime).TotalSeconds);
            
            SendTouchMove(new TouchMove(e.Location, _isDown, velocity));

            if (_isDown)
            {
                // Check for swipe
                if (!_isInSwipe && velocity > TouchSwipeVelocityThreshold)
                {
                    _isInSwipe = true;
                    _startHighVelocityPoint = e.Location;
                }
                else if (_isInSwipe && velocity < TouchSwipeVelocityThreshold)
                    _isInSwipe = false;
                else if (_isInSwipe)
                {

                    
                    // TODO: Chewck for angle. 
                    // HACK: the following code.
                    float x = _startHighVelocityPoint.X - e.X;
                    float y = _startHighVelocityPoint.Y - e.Y;
                    System.Diagnostics.Debug.WriteLine("IS IN SWIPE X: " + x.ToString()  + " y: " + y.ToString());
                    if (Math.Abs(x) > Math.Abs(y))
                    {
                        if (Math.Abs(x) > TouchSwipeDistanceThreshold)
                        {

                            _isInSwipe = false;
                            SendTouchGesture(new TouchGesture(
                                x > 0
                                ? GestureType.SwipeLeft
                                : GestureType.SwipeRight
                                , e.Location));
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
                                , e.Location));
                        }
                    }
                }
            }

            _previousPointTime = DateTime.Now;
            _previousPoint = e.Location;
        }

        private void SendTouchGesture(TouchGesture pTouchGesture)
        {
            ObservablTouchActions.ObsTouchGesture.PushTouchGesture(pTouchGesture);
            System.Diagnostics.Debug.WriteLine("Gesture: " + pTouchGesture.Gesture.ToString() + " at " + pTouchGesture.X.ToString() + "," + pTouchGesture.Y.ToString());
        }

        private void SendTouchMove(TouchMove pTouchMove)
        {
            ObservablTouchActions.ObsTouchMove.PushTouchMove(pTouchMove);
            System.Diagnostics.Debug.WriteLine("Velocity: " + pTouchMove.Velocity.ToString() + ", down: " + pTouchMove.TouchDown.ToString() + " at " + pTouchMove.X.ToString() + "," + pTouchMove.Y.ToString());
        }
    }
}
