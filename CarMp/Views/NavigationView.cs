using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMp.ViewControls;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMp.Views
{
    public abstract class NavigationView : D2DView, ISkinable
    {
        private const string XPATH_NAVIGATION_BACKGROUND = "BackgroundImg";
        private const string XPATH_NAVIGATION_NODE = "NavigationBar";
        private const string XPATH_BUTTONS = "Buttons/*";

        public NavigationView(SizeF pWindowSize)
            : base(pWindowSize) { }

        private CarMp.Direct2D.BitmapData _navigationBarBackgroundBitmapData;
        private D2DBitmap _navigationBarBackgroundBitmap = null;
        
        public void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            _navigationBarBackgroundBitmap = null;
            XmlNode navNode = pSkinNode.SelectSingleNode(XPATH_NAVIGATION_NODE);
            if (navNode != null)
            {
                foreach (XmlNode buttonNode in navNode.SelectNodes(XPATH_BUTTONS))
                {
                    GraphicalButton button = new GraphicalButton();
                    button.ApplySkin(buttonNode, pSkinPath);
                    AddViewControl(button);
                    button.StartRender();
                    switch (buttonNode.Name)
                    {
                        case D2DViewFactory.MEDIA:
                            button.Click += new EventHandler((sender, e) => AppMain.AppFormHost.ShowView(D2DViewFactory.MEDIA));
                            break;
                        case D2DViewFactory.HOME:
                            button.Click += new EventHandler((sender, e) => AppMain.AppFormHost.ShowView(D2DViewFactory.HOME));
                            break;
                    }
                }

                XmlNode xmlNode = navNode.SelectSingleNode(XPATH_NAVIGATION_BACKGROUND);
                if (xmlNode != null)
                    _navigationBarBackgroundBitmapData = new Direct2D.BitmapData(System.IO.Path.Combine(pSkinPath, xmlNode.InnerText));

            }
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if (_navigationBarBackgroundBitmap == null
                && _navigationBarBackgroundBitmapData.Data != null)
            {
                _navigationBarBackgroundBitmap = Direct2D.GetBitmap(_navigationBarBackgroundBitmapData, pRenderTarget.Renderer);
            }

            if(_navigationBarBackgroundBitmap != null)
                pRenderTarget.DrawBitmap(_navigationBarBackgroundBitmap, new RectF(0, 0, _navigationBarBackgroundBitmapData.Width, _navigationBarBackgroundBitmapData.Height));
        }
    }
}
