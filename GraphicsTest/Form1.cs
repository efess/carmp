using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CarMP.Graphics;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Implementation.OpenGL;
using CarMP.Graphics.Implementation.SFML;

namespace GraphicsTest
{
    public partial class Form1 : Form
    {
        private DateTime _fpsCalcDate;
        private int _fpsCalcFramesCurrent;
        private IRenderer _renderer;
        private ControlForm _controlForm;
        public Form1()
        {
            //_renderer = new RendererFactory().GetRenderer("opengl", this.Handle);
            InitializeComponent();
                
            _controlForm = new ControlForm();
            _controlForm.StartPosition = FormStartPosition.Manual;
            _controlForm.Location = new System.Drawing.Point(Location.X + Size.Width, Location.Y);
            _controlForm.Show();

            _renderer = new SFMLRenderer();
            
            // OPEN GL RENDERER LOL
            //_renderer = new OpenGLRenderer();

            //var renderDelegate = new CarMP.Graphics.Implementation.OpenGL.OpenGLRenderer.RenderEventHandler(RenderEvent);

            //new Action(() => ((OpenGLRenderer)_renderer).DoesItWork(renderDelegate)).BeginInvoke(null, null);
            
            Action renderingLoop = new Action(() => RenderingLoop());
            renderingLoop.BeginInvoke(null, null);
        }

        private static IImage imageResource;

        private void RenderEvent()
        {
            _renderer.DrawRectangle(new OpenGLBrush
            {
                Color = new Color(Color.Gray, 1f)
            }, new Rectangle(20,20,200,500),50);

            _renderer.DrawRectangle(new OpenGLBrush
            {
                Color = new Color(Color.White, 1f)
            }, new Rectangle(30,30, 50,50), 18);

            _renderer.DrawRectangle(new OpenGLBrush
            {
                Color = new Color(Color.White, .75f)
            }, new Rectangle(30, 130, 50, 50), 18);

            _renderer.DrawRectangle(new OpenGLBrush
            {
                Color = new Color(Color.White, .55f)
            }, new Rectangle(30, 230, 50, 50), 18);

            _renderer.DrawRectangle(new OpenGLBrush
            {
                Color = new Color(Color.White, .25f)
            }, new Rectangle(30, 330, 50, 50), 18);


            //if (imageResource == null)
            //    imageResource = _renderer.CreateImage(@"C:\source\CarMp\trunk\Images\Skins\BMW\pause.png");

            //_renderer.DrawImage(new Rectangle(100, 30, 50, 51), imageResource, 1);

            // DrawDirect2D();
            //_fpsCalcFramesCurrent++;
            //if (DateTime.Now > _fpsCalcDate)
            //{
            //    _controlForm.FPS = _fpsCalcFramesCurrent;
            //    _fpsCalcFramesCurrent = 0;
            //    _fpsCalcDate = DateTime.Now.AddSeconds(1);
            //}
            
        }

        private void RenderingLoop()
        {
            System.Threading.Thread.CurrentThread.Name = "Rendering Loop";
            while (true)
            {
                DoRender();
                _fpsCalcFramesCurrent++;
                if (DateTime.Now > _fpsCalcDate)
                {
                    _controlForm.FPS = _fpsCalcFramesCurrent;
                    _fpsCalcFramesCurrent = 0;
                    _fpsCalcDate = DateTime.Now.AddSeconds(1);
                }

                System.Threading.Thread.Sleep(10);
            }
        }


        IStringLayout stringLayout;
        IBrush stringBrush;

        private void DoRender()
        {
            try
            {
                //if (!_renderTarget.IsOccluded)
                //{
                _renderer.BeginDraw();

                _renderer.Clear(CarMP.Graphics.Color.Black);

                RenderEvent();

                //if(stringBrush == null)
                //    stringBrush = _renderer.CreateBrush(CarMP.Graphics.Color.RoyalBlue);
                //if (stringLayout == null)
                //    stringLayout = _renderer.CreateStringLayout("Hello World!", "Arial", 25);

                //_renderer.DrawRectangle(stringBrush, new Rectangle(1, 1, this.Width - 2, this.Height - 2), 1);
                //_renderer.DrawString(new Rectangle(50, 50, 100, 50), stringLayout, stringBrush);
                
                //for (int i = _overlayViewControls.Count - 1;
                //    i >= 0;
                //    i--)
                //{
                //    _overlayViewControls[i].Render(_renderTarget);
                //}

                _renderer.EndDraw();

                //this.Invalidate();
                //}
            }
            catch (Exception ex)
            {
                _renderer.Flush();
                throw;
            }
        }


        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            //if (_currentView is D2DView)
            //{
            //    (_currentView as D2DView).OnSizeChanged(this, e);
            //}
        }


    }
}
