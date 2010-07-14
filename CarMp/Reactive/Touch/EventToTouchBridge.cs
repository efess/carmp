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

        private bool _isInSwipe;
        private PointF _downPoint;
        private DateTime _downTime;
        private PointF _previousPoint;
        private PointF _startHighVelocityPoint;
        private DateTime _previousPointTime;
        private VelocityAggregator _velocity;

        private System.Threading.Timer _velocityTimer;

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
            TouchSwipeDistanceThreshold = 50;
            TouchSwipeVelocityThreshold = 1000;

            _velocity = new VelocityAggregator(3);

            CreateEventSubscriptions();
            ObservablTouchActions = new TouchObservables();
            _velocityTimer = new System.Threading.Timer(new System.Threading.TimerCallback(ProcessVelocity));
        }

        public void CreateEventSubscriptions()
        {
            _control.MouseUp += (sender, e) => ProcessMouseUp(e);
            _control.MouseDown += (sender, e) => ProcessMouseDown(e);
            _control.MouseMove += (sender, e) => ProcessMouseMove(e);
        }

        private void ProcessMouseUp(MouseEventArgs e)
        {
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
            _downPoint = e.Location;
            _downTime = DateTime.Now;
        }

        private void ProcessMouseMove(MouseEventArgs e)
        {
            DateTime dt = DateTime.Now;

            if (dt == _previousPointTime) return;

            bool mouseDown = e.Button == MouseButtons.Left;

            _velocity.VelocityNow = LinearMath.DistanceBetweenTwoPoint(e.Location, _previousPoint)
                / (float)((dt - _previousPointTime).TotalSeconds);
            
            float velocityNow = _velocity.GetVelocity;

            // WORKS FUCKING SWEET!
            //... Except direction is fucked up. Spoke too soon...
            //System.Diagnostics.Debug.WriteLine("MouseMove- velocity: " + velocityNow + "  now: " + e.X.ToString() + "," + e.Y.ToString() + " previous: " + _previousPoint.X.ToString() + "," + _previousPoint.Y.ToString() + " " + e.Button.ToString());

            SendTouchMove(new TouchMove(e.Location, mouseDown, velocityNow));

            if (mouseDown)
            {
                // Check for swipe
                if (!_isInSwipe && velocityNow > TouchSwipeVelocityThreshold)
                {
                    _isInSwipe = true;
                    _startHighVelocityPoint = e.Location;
                }
                else if (_isInSwipe && velocityNow < TouchSwipeVelocityThreshold)
                    _isInSwipe = false;
                else if (_isInSwipe)
                {

                    
                    // TODO: Chewck for angle. 
                    // HACK: the following code.
                    float x = _startHighVelocityPoint.X - e.X;
                    float y = _startHighVelocityPoint.Y - e.Y;
                    DebugHandler.DebugPrint("IS IN SWIPE X: " + x.ToString()  + " y: " + y.ToString());
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

            _previousPointTime = dt;
            _previousPoint = e.Location;
        }

        private void ProcessVelocity(object pStateObject)
        {

            ////////////////////////
            // WHEN MOOUSE STOPS, VERY ABRUPTLY, Must maintain a decent velocity somehow?
            //

        }


        private void SendTouchGesture(TouchGesture pTouchGesture)
        {
            ObservablTouchActions.ObsTouchGesture.PushTouchGesture(pTouchGesture);
            DebugHandler.DebugPrint("Gesture: " + pTouchGesture.Gesture.ToString() + " at " + pTouchGesture.X.ToString() + "," + pTouchGesture.Y.ToString());
        }

        private void SendTouchMove(TouchMove pTouchMove)
        {
            ObservablTouchActions.ObsTouchMove.PushTouchMove(pTouchMove);
            DebugHandler.DebugPrint("Velocity: " + pTouchMove.Velocity.ToString() + ", down: " + pTouchMove.TouchDown.ToString() + " at " + pTouchMove.X.ToString() + "," + pTouchMove.Y.ToString());
        }
    }
}
