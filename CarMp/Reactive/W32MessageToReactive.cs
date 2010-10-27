using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMP.Reactive.KeyInput;
using CarMP.Win32;
using System.Diagnostics;
using CarMP.Reactive.Touch;
using CarMP.Callbacks;

namespace CarMP.Reactive
{
    public class W32MessageToReactive
    {
        //**** Mouse State Vars

        private bool _isDownHolding;
        private bool _isDown;
        private bool _isInSwipe;
        private Point2F _downPoint;
        private DateTime _downTime;
        private Point2F _previousPoint;
        private Point2F _startHighVelocityPoint;
        private DateTime _previousPointTime;
        private System.Threading.Timer _waitTimer;

        private VelocityAggregator _velocityAgg;

        //**** Feel Control Properties / Parameters

        public int TouchDownClickThreshold { get; set; }
        public int TouchDownClickDistanceTolerance { get; set; }
        public int TouchSwipeDistanceThreshold { get; set; }
        public int TouchSwipeVelocityThreshold { get; set; }
        public int TouchDownHoldThreshold { get; set; }

        //****

        private IMessageHookable _hookedPump;
        private Control _control;
        public readonly Observables ObservableActions;

        private Keys[] supportedKeys = new Keys[] {
            Keys.Left, Keys.Right, Keys.Up, Keys.Down,
                            Keys.Back, Keys.Delete};

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
            TouchDownHoldThreshold = 1000;
            TouchDownClickThreshold = 200;
            TouchDownClickDistanceTolerance = 5;
            TouchSwipeDistanceThreshold = 50;
            TouchSwipeVelocityThreshold = 1000;

            _velocityAgg = new VelocityAggregator(3);

            ObservableActions = new Observables();
        }

        public void ProcessMessage(ref Message pMessage)
        {
            switch (pMessage.Msg)
            {
                case WindowsMessages.WM_CHAR:
                    {
                        char iKey = (char)pMessage.WParam;
                        Keys key = 0;//(Keys)pMessage.WParam;

                        //Debug.WriteLine("key: " + key.ToString() + ", " + iKey.ToString() + ", " + ((int)((Keys)iKey & Keys.KeyCode)).ToString());
                        
                        ProcessKeyPress(iKey, key);
                    }
                    break;
                case WindowsMessages.WM_KEYDOWN:
                    {
                        bool _upper = Win32Helpers.GetKeyState((int)Keys.CapsLock) != 0
                            ^ Win32Helpers.GetKeyState((int)Keys.ShiftKey) < 0;
                        
                        bool _ctrl = Win32Helpers.GetKeyState((int)Keys.ControlKey) != 0;

                        int iKey =(int) Win32Helpers.MapVirtualKey((uint)pMessage.WParam, 2);
                        
                        Keys key = (Keys)pMessage.WParam;
                        if (supportedKeys.Contains(key))
                        {
                            //Debug.WriteLine("Shift: " + _upper + " KeyDown " + key.ToString() + ", " + iKey.ToString() + ", " + ((int)((Keys)iKey & Keys.KeyCode)).ToString());

                            ProcessKeyPress((char)TranslateKey(iKey, _ctrl, _upper), key);
                        }
                    }
                    break;
                case WindowsMessages.WM_MOUSEMOVE:
                    ProcessMouseMove(GetMouseCoordFromLParam((int)pMessage.LParam));
                    break;
                case WindowsMessages.WM_LBUTTONDOWN:
                    ProcessMouseDown(GetMouseCoordFromLParam((int)pMessage.LParam));
                    break;
                case WindowsMessages.WM_LBUTTONUP:
                    ProcessMouseUp(GetMouseCoordFromLParam((int)pMessage.LParam));
                    break;
                    
            }
        }

        private int TranslateKey(int pChar, bool pControl, bool pShift)
        {
            if (pChar >= 65 && pChar <= 90)
                if (!pShift)
                    return pChar + 32;
            if(pShift)
                switch (pChar)
                {
                    case 59:
                        return 58;

                }


            return pChar;
        }
        
        private void ProcessKeyPress(char pChar, Keys pKeyChar)
        {
            SendKeyInput(new Key(pChar, pKeyChar));
        }

        private void ProcessMouseUp(Point2F pMouseCoordinate)
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

        private void ProcessMouseDown(Point2F pMouseCoordinate)
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

        private void ProcessMouseMove(Point2F pMouseCoordinate)
        {
            DateTime dt = DateTime.Now;
            Point2F mousePoint = pMouseCoordinate;

            if (dt == _previousPointTime) return;

            bool mouseDown = _isDown;
            float seconds = (float)(dt - _previousPointTime).TotalSeconds;

            float xVelocity =Math.Abs((mousePoint.X - _previousPoint.X)
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

        private Point2F GetMouseCoordFromLParam(int pLParam)
        {
            return new Point2F((pLParam & 0xFFFF), pLParam >> 16);
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
            //DebugHandler.DebugPrint("Gesture: " + pTouchGesture.Gesture.ToString() + " at " + pTouchGesture.X.ToString() + "," + pTouchGesture.Y.ToString());
        }

        private void SendTouchMove(TouchMove pTouchMove)
        {
            ObservableActions.ObsTouchMove.PushTouchMove(pTouchMove);
            //DebugHandler.DebugPrint("Velocity: " + pTouchMove.Velocity.VelocityD.ToString() + ", down: " + pTouchMove.TouchDown.ToString() + " at " + pTouchMove.X.ToString() + "," + pTouchMove.Y.ToString());
        }

        private void StopWaitTimer()
        {
            _waitTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

    }
    public enum AsciiChar
    {
    }
}
