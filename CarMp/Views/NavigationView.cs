using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.ViewControls;
using CarMP.Direct2D;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP.Views
{
    public abstract class NavigationView : D2DView, ISkinable
    {
        private const string XPATH_NAVIGATION_BACKGROUND = "BackgroundImg";
        private const string XPATH_NAVIGATION_NODE = "NavigationBar";
        private const string XPATH_BUTTONS = "Buttons/*";

        public NavigationView(SizeF pWindowSize)
            : base(pWindowSize) { }

        private CarMP.Direct2D.BitmapData _navigationBarBackgroundBitmapData;
        private D2DBitmap _navigationBarBackgroundBitmap = null;
        
        public virtual void ApplySkin(XmlNode pSkinNode, string pSkinPath)
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
                        case D2DViewFactory.HOME:
                        case D2DViewFactory.OPTIONS:
                        case D2DViewFactory.NAVIGATION:
                            {
                                string localVar = buttonNode.Name;
                                button.Click += (sender, e) => AppMain.AppFormHost.ShowView(localVar);
                                break;
                            }
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
                _navigationBarBackgroundBitmap = D2DStatic.GetBitmap(_navigationBarBackgroundBitmapData, pRenderTarget.Renderer);
            }

            if(_navigationBarBackgroundBitmap != null)
                pRenderTarget.DrawBitmap(_navigationBarBackgroundBitmap, new RectF(0, 0, _navigationBarBackgroundBitmapData.Width, _navigationBarBackgroundBitmapData.Height));
        }
    }
}
