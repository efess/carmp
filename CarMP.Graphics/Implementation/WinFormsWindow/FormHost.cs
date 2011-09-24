using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics.Geometry;
using System.Runtime.InteropServices;

namespace CarMP.Implementation.WinFormsWindow
{
    public partial class FormHost : Form, IWindow
    {
        [DllImport("user32.dll")]
        public static extern short GetKeyState(int pKey);

        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint uCode, uint uMapType);

        private System.Windows.Forms.Keys[] supportedKeys = new System.Windows.Forms.Keys[] {
            System.Windows.Forms.Keys.Left, 
            System.Windows.Forms.Keys.Right, 
            System.Windows.Forms.Keys.Up, 
            System.Windows.Forms.Keys.Down,
            System.Windows.Forms.Keys.Back, 
            System.Windows.Forms.Keys.Delete
        };

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
        }

        public FormHost()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            InitializeComponent();
        }

        public void ProcessMessage(ref Message pMessage)
        {
            switch (pMessage.Msg)
            {
                case WindowsMessages.WM_CHAR:
                    {
                        char iKey = (char)pMessage.WParam;
                        System.Windows.Forms.Keys key = 0;//(Keys)pMessage.WParam;

                        //Debug.WriteLine("key: " + key.ToString() + ", " + iKey.ToString() + ", " + ((int)((Keys)iKey & Keys.KeyCode)).ToString());
                        if (_processKeyPress != null)
                            _processKeyPress(iKey, TranslateWindowsKey(key));
                    }
                    break;
                case WindowsMessages.WM_KEYDOWN:
                    {
                        bool _upper = GetKeyState((int)Keys.CapsLock) != 0
                            ^ GetKeyState((int)Keys.ShiftKey) < 0;

                        bool _ctrl = GetKeyState((int)Keys.ControlKey) != 0;

                        int iKey = (int)MapVirtualKey((uint)pMessage.WParam, 2);

                        Keys key = (Keys)pMessage.WParam;
                        if (supportedKeys.Contains(key))
                        {
                            //Debug.WriteLine("Shift: " + _upper + " KeyDown " + key.ToString() + ", " + iKey.ToString() + ", " + ((int)((Keys)iKey & Keys.KeyCode)).ToString());
                            if (_processKeyPress != null)
                                _processKeyPress((char)TranslateKey(iKey, _ctrl, _upper), TranslateWindowsKey(key));
                        }
                    }
                    break;
                case WindowsMessages.WM_MOUSEMOVE:
                    if (_processMouseMove != null)
                        _processMouseMove(GetMouseCoordFromLParam((int)pMessage.LParam));
                    break;
                case WindowsMessages.WM_LBUTTONDOWN:
                    if (_processMouseDown != null)
                        _processMouseDown(GetMouseCoordFromLParam((int)pMessage.LParam));
                    break;
                case WindowsMessages.WM_LBUTTONUP:
                    if (_processMouseUp != null)
                        _processMouseUp(GetMouseCoordFromLParam((int)pMessage.LParam));
                    break;
            }
        }

        private int TranslateKey(int pChar, bool pControl, bool pShift)
        {
            if (pChar >= 65 && pChar <= 90)
                if (!pShift)
                    return pChar + 32;
            if (pShift)
                switch (pChar)
                {
                    case 59:
                        return 58;

                }


            return pChar;
        }

        private CarMP.Graphics.Keys TranslateWindowsKey(System.Windows.Forms.Keys pKey)
        {
            return CarMP.Graphics.Keys.A;// TODO!!
        }

        public void SetProcessKeyPress(Action<char, CarMP.Graphics.Keys> pCallback) { _processKeyPress = pCallback; }
        public void SetProcessMouseMove(Action<Point> pCallback) { _processMouseMove = pCallback; }
        public void SetProcessMouseDown(Action<Point> pCallback) { _processMouseDown = pCallback; }
        public void SetProcessMouseUp(Action<Point> pCallback) { _processMouseUp = pCallback; }

        Action<char, CarMP.Graphics.Keys> _processKeyPress;
        Action<Point> _processMouseMove;
        Action<Point> _processMouseDown;
        Action<Point> _processMouseUp;

        public IRenderer Renderer
        {
            get
            {
                return null;
            }
        }

        public void CreateWindow(Point pWindowLocation, Size pWindowSize)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point((int)pWindowLocation.X, (int)pWindowLocation.Y);
            this.Size = new System.Drawing.Size((int)pWindowSize.Width, (int)pWindowSize.Height);

            this.Show();
        }

        private Point GetMouseCoordFromLParam(int pLParam)
        {
            return new Point((pLParam & 0xFFFF), pLParam >> 16);
        }        

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnSizeChanged(EventArgs e)
        {
        }
    }
}
