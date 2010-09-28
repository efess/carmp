using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using System.Xml;

namespace CarMP.ViewControls
{
    public class ViewControlCommonBase : D2DViewControl, ISkinable, IDisposable
    {
        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";
        private const string XPATH_BACKGROUND_COLOR = "BackgroundColor";
        private const string XPATH_BOUNDS = "Bounds";

        private D2DBitmap _backGroundBitmap;
        private Direct2D.BitmapData _backGroundBitmapData;
        private ColorF _backgroundColor;
        private SolidColorBrush _backgroundColorBrush;

        public void Dispose()
        {
            if (_backgroundColorBrush != null) _backgroundColorBrush.Dispose();
            if (_backGroundBitmap != null) _backGroundBitmap.Dispose();
        }

        private bool _hasColor = false;

        public virtual void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            if (SkinningHelper.XmlRectangleFEntry(XPATH_BOUNDS, pXmlNode, ref _bounds))
                OnSizeChanged(null, null);
            if (SkinningHelper.XmlBitmapEntry(XPATH_BACKGROUND_IMAGE, pXmlNode, pSkinPath, ref _backGroundBitmapData))
                _backGroundBitmap = null;

            _hasColor = SkinningHelper.XmlColorEntry(XPATH_BACKGROUND_COLOR, pXmlNode, ref _backgroundColor);
            if (_hasColor)
                _backgroundColorBrush = null;
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if (_hasColor &&
                _backgroundColorBrush == null)
            {
                _backgroundColorBrush = pRenderTarget.Renderer.CreateSolidColorBrush(_backgroundColor);
            }
            if (_backgroundColorBrush != null)
            {
                pRenderTarget.FillRectangle(_backgroundColorBrush, new RectF(0,0,_bounds.Width,_bounds.Height));
            }

            if (_backGroundBitmap == null
                && _backGroundBitmapData.Data != null)
            {
                _backGroundBitmap = D2DStatic.GetBitmap(_backGroundBitmapData, pRenderTarget.Renderer);
            }

            if (_backGroundBitmap != null)
            {
                pRenderTarget.DrawBitmap(_backGroundBitmap, new RectF(0, 0, Bounds.Width, Bounds.Height));
            }
        }
    }
        
}
