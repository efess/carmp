using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct2D;
using System.Xml;
using CarMp.ViewControls;

namespace CarMp.Views
{
    public class HomeView : D2DView, ISkinable
    {
        private const string XPATH_BUTTONS = "Buttons/*";
        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";

        private SlimDX.Direct2D.Bitmap BackgroundImage = null;

        private Direct2D.BitmapData _backgroundImage;
        
        internal HomeView(System.Drawing.Size pWindowSize)
            : base(pWindowSize) {}

        public void ApplySkin(XmlNode pSkinViewNode, string pSkinPath)
        {
            _viewControls.Clear();

            XmlNode xmlNode = pSkinViewNode.SelectSingleNode(XPATH_BACKGROUND_IMAGE);
            if (xmlNode != null)
                _backgroundImage = new Direct2D.BitmapData(System.IO.Path.Combine(pSkinPath, xmlNode.InnerText));

            foreach (XmlNode buttonNode in pSkinViewNode.SelectNodes(XPATH_BUTTONS))
            {
                GraphicalButton button = new GraphicalButton();
                button.ApplySkin(buttonNode, pSkinPath);
                AddViewControl(button);
                button.StartRender();
                switch (buttonNode.Name)
                {
                    case D2DViewFactory.DIGITAL_AUDIO:
                        button.Click += new EventHandler((sender, e) => ApplicationMain.AppFormHost.ShowView(D2DViewFactory.DIGITAL_AUDIO));
                        break;
                }
            }
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if (BackgroundImage == null
                && _backgroundImage != null)
            {
                BackgroundImage = new SlimDX.Direct2D.Bitmap(pRenderTarget.Renderer, new System.Drawing.Size(_backgroundImage.Width, _backgroundImage.Height), new SlimDX.DataStream(_backgroundImage.Data, true, false), _backgroundImage.Stride, _backgroundImage.BitmapProperties);
            }
            if (BackgroundImage != null)
            {
                pRenderTarget.DrawBitmap(BackgroundImage, new System.Drawing.RectangleF(0,0,_backgroundImage.Width, _backgroundImage.Height));
            }
        }
    }
}
