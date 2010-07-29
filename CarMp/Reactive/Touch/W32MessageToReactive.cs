using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMp.Reactive.KeyInput;
using CarMp.Win32;
using System.Diagnostics;

namespace CarMp.Reactive.Touch
{
    public class W32MessageToReactive
    {
        //**** Mouse State Vars

        private bool _isInSwipe;
        private Point2F _downPoint;
        private DateTime _downTime;
        private Point2F _previousPoint;
        private Point2F _startHighVelocityPoint;
        private DateTime _previousPointTime;

        private VelocityAggregator _velocityAgg;

        private System.Threading.Timer _velocityTimer;

        //**** Feel Control Properties / Parameters

        public int TouchDownClickThreshold { get; set; }
        public int TouchDownClickDistanceTolerance { get; set; }
        public int TouchSwipeDistanceThreshold { get; set; }
        public int TouchSwipeVelocityThreshold { get; set; }

        //****

        //**** Keyboard Bools

        private bool _shift = false;
        private bool _ctrl = false;
        private bool _alt = false;

        //*****
        private IMessageHookable _hookedPump;
        private Control _control;
        public readonly Observables ObservableActions;

        public W32MessageToReactive(Control pControl)
        {
            if (pControl == null)
                throw new ArgumentNullException("pControl");

            if (pControl is IMessageHookable)
            {
                (pControl as IMessageHookable).MessagePump +=
                    new Messenger(ProcessMessage); 
            }
            _control = pControl;

            // Defaults
            TouchDownClickThreshold = 200;
            TouchDownClickDistanceTolerance = 5;
            TouchSwipeDistanceThreshold = 50;
            TouchSwipeVelocityThreshold = 1000;

            _velocityAgg = new VelocityAggregator(3);

            CreateEventSubscriptions();
            ObservableActions = new Observables();
            _velocityTimer = new System.Threading.Timer(new System.Threading.TimerCallback(ProcessVelocity));
        }

        public void ProcessMessage(ref Message pMessage)
        {
            switch (pMessage.Msg)
            {
                case WindowsMessages.WM_KEYDOWN:
                    {
                        short test = Win32Helpers.GetKeyState((int)Keys.CapsLock);
                        short TEST2 = Win32Helpers.GetKeyState((int)Keys.ShiftKey);
                        bool _upper = Win32Helpers.GetKeyState((int)Keys.CapsLock) != 0
                            ^ Win32Helpers.GetKeyState((int)Keys.ShiftKey) != 0;

                        int iKey =(int) Win32Helpers.MapVirtualKey((uint)pMessage.WParam, 2);
                        Keys key = (Keys)pMessage.WParam;
                        Debug.WriteLine("Shift: " + _upper + " KeyDown " + key.ToString() + ", " + iKey.ToString() + ", " + ((int)((Keys)iKey & Keys.KeyCode)).ToString());
                        switch(key)
                        {
                            case Keys.ShiftKey:
                                _shift = true;
                                break;;
                            case Keys.ControlKey:
                                _ctrl = true;
                                break;
                            case Keys.Alt:
                                _alt = true;
                                break;
                            default:
                                ProcessKeyPress((char)TranslateKey(iKey), key);
                                break;
                        }
                    }
                    break;
                
            }
        }

        private int TranslateKey(int pChar)
        {
            if (pChar >= 65 && pChar <= 90)
                if (!_shift)
                    return pChar + 32;
            return pChar;
        }

        public void CreateEventSubscriptions()
        {
            //_control.KeyPress += (sender, e) => ProcessKeyPress(e);
            _control.MouseUp += (sender, e) => ProcessMouseUp(e);
            _control.MouseDown += (sender, e) => ProcessMouseDown(e);
            _control.MouseMove += (sender, e) => ProcessMouseMove(e);
        }

        private void ProcessKeyPress(char pChar, Keys pKeyChar)
        {
            SendKeyInput(new Key(pChar, pKeyChar));
        }

        private void ProcessMouseUp(MouseEventArgs e)
        {
            // Check for Click
            if (TouchDownClickThreshold > (DateTime.Now - _downTime).TotalMilliseconds
                && Math.Abs(_downPoint.Y - e.Y) < TouchDownClickDistanceTolerance)
            {
                SendTouchGesture(new TouchGesture(GestureType.Click, new Point2F(e.Location.X, e.Location.Y)));
                return;
            }            
        }

        private void ProcessMouseDown(MouseEventArgs e)
        {
            _isInSwipe = false;
            _downPoint = new Point2F(e.X, e.Y);
            _downTime = DateTime.Now;
        }

        private void ProcessMouseMove(MouseEventArgs e)
        {
            DateTime dt = DateTime.Now;
            Point2F mousePoint = new Point2F(e.X, e.Y);

            if (dt == _previousPointTime) return;

            bool mouseDown = e.Button == MouseButtons.Left;
            float seconds = (float)(dt - _previousPointTime).TotalSeconds;

            float xVelocity =Math.Abs((mousePoint.X - _previousPoint.X)
                / seconds);
            
            float yVelocity = Math.Abs((mousePoint.Y - _previousPoint.Y)
                / seconds);
                        
            float directionalVelocity = LinearMath.DistanceBetweenTwoPoint(mousePoint, _previousPoint)
                / seconds;

            _velocityAgg.VelocityNow = new Velocity(xVelocity, yVelocity, directionalVelocity);
            Velocity velocityNow = _velocityAgg.GetVelocity;

            // WORKS FUCKING SWEET!
            //... Except direction is fucked up. Spoke too soon...
            //System.Diagnostics.Debug.WriteLine("MouseMove- velocity: " + velocityNow + "  now: " + e.X.ToString() + "," + e.Y.ToString() + " previous: " + _previousPoint.X.ToString() + "," + _previousPoint.Y.ToString() + " " + e.Button.ToString());

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

        private void SendKeyInput(Key pKeyInput)
        {
            ObservableActions.ObsKeyInput.PushKeyInput(pKeyInput);
        }

        private void SendTouchGesture(TouchGesture pTouchGesture)
        {
            ObservableActions.ObsTouchGesture.PushTouchGesture(pTouchGesture);
            DebugHandler.DebugPrint("Gesture: " + pTouchGesture.Gesture.ToString() + " at " + pTouchGesture.X.ToString() + "," + pTouchGesture.Y.ToString());
        }

        private void SendTouchMove(TouchMove pTouchMove)
        {
            ObservableActions.ObsTouchMove.PushTouchMove(pTouchMove);
            DebugHandler.DebugPrint("Velocity: " + pTouchMove.Velocity.VelocityD.ToString() + ", down: " + pTouchMove.TouchDown.ToString() + " at " + pTouchMove.X.ToString() + "," + pTouchMove.Y.ToString());
        }
    }
}
