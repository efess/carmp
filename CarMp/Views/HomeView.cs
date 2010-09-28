using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.ViewControls;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP.Views
{
    public class HomeView : NavigationView, ISkinable
    {
        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";

        private D2DBitmap BackgroundImage = null;

        private Direct2D.BitmapData _backgroundImage;
        
        internal HomeView(SizeF pWindowSize)
            : base(pWindowSize) {}

        public new void ApplySkin(XmlNode pSkinViewNode, string pSkinPath)
        {
            Clear();

            BackgroundImage = null;
            XmlNode xmlNode = pSkinViewNode.SelectSingleNode(XPATH_BACKGROUND_IMAGE);
            if (xmlNode != null)
                _backgroundImage = new Direct2D.BitmapData(System.IO.Path.Combine(pSkinPath, xmlNode.InnerText));

            base.ApplySkin(pSkinViewNode, pSkinPath);
        }

        public override string Name
        {
            get { return D2DViewFactory.HOME; }
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if (BackgroundImage == null
                && _backgroundImage.Data != null)
            {
                BackgroundImage = D2DStatic.GetBitmap(_backgroundImage, pRenderTarget.Renderer);
            }
            if (BackgroundImage != null)
            {
                pRenderTarget.DrawBitmap(BackgroundImage, new RectF(0,0,_backgroundImage.Width, _backgroundImage.Height));
            }
            base.OnRender(pRenderTarget);
        }
    }
}
