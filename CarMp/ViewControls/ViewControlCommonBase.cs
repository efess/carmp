using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;
using System.Xml;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics;
using CarMP.Helpers;

namespace CarMP.ViewControls
{
    public abstract class ViewControlCommonBase : D2DViewControl, ISkinable, IDisposable
    {
        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";
        private const string XPATH_BACKGROUND_COLOR = "BackgroundColor";
        private const string XPATH_BOUNDS = "Bounds";

        private IImage _backGroundBitmap;
        private string _backGroundBitmapPath;
        private Color _backgroundColor;
        private IBrush _backgroundColorBrush;

        public virtual void Dispose()
        {
            if (_backgroundColorBrush != null) 
                Helpers.GraphicsHelper.DisposeIfImplementsIDisposable(_backgroundColorBrush);
            if (_backGroundBitmap != null) 
                Helpers.GraphicsHelper.DisposeIfImplementsIDisposable(_backGroundBitmap);
        }

        public virtual string Name { get; private set; }

        private bool _hasColor = false;

        public virtual void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            Clear();
            
            Name = pXmlNode.Name;

            if (SkinningHelper.XmlRectangleEntry(XPATH_BOUNDS, pXmlNode, ref _bounds))
                OnSizeChanged(null, null);
            if (SkinningHelper.XmlValidFilePath(XPATH_BACKGROUND_IMAGE, pXmlNode, pSkinPath, ref _backGroundBitmapPath))
                _backGroundBitmap = null;

            _hasColor = SkinningHelper.XmlColorEntry(XPATH_BACKGROUND_COLOR, pXmlNode, ref _backgroundColor);
            if (_hasColor)
                _backgroundColorBrush = null;


            foreach (XmlNode childNode in pXmlNode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    var viewControl = ViewControlFactory.GetViewControlAndApplySkin(childNode.Name, pSkinPath, childNode);
                    if (viewControl != null)
                        AddViewControl(viewControl);
                }

                //if (viewControl is ThermometerProgressBar)
                //{
                //    _progressBar = viewControl as ThermometerProgressBar; ;
                //}
            }
        }

        public virtual void SetBackground(string pBitmapPath)
        {
            _backGroundBitmapPath = pBitmapPath;
            _backGroundBitmap = null;
        }

        protected override void OnRender(IRenderer pRenderer)
        {
            if (_hasColor &&
                _backgroundColorBrush == null)
            {
                _backgroundColorBrush = pRenderer.CreateBrush(_backgroundColor);
            }

            if (_backgroundColorBrush != null)
            {
                pRenderer.FillRectangle(_backgroundColorBrush, new Rectangle(0,0,_bounds.Width,_bounds.Height));
            }

            if (_backGroundBitmap == null
                && !string.IsNullOrEmpty(_backGroundBitmapPath))
            {
                _backGroundBitmap = pRenderer.CreateImage(_backGroundBitmapPath);
            }

            var background = _backGroundBitmap;
            if (background != null)
            {
                pRenderer.DrawImage(new Rectangle(0, 0, Bounds.Width, Bounds.Height), background, 1F);
            }
        }
    }
        
}
