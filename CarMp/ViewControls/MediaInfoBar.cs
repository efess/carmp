using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMp.ViewControls
{
    public class MediaInfoBar : AnimationContainer, ISkinable
    {
        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";
        private const string XPATH_TEXT_MEDIA_CREATOR = "MediaCreator";
        private const string XPATH_TEXT_MEDIA_TITLE = "MediaTitle";
        private const string XPATH_ANIMATION_POINT = "AnimationPath/*";

        private Direct2D.BitmapData _backgroundBitmapData;
        private SlimDX.Direct2D.Bitmap _backgroundImage = null;

        public void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            _viewControls.Clear();

            XmlNode node = pSkinNode.SelectSingleNode(XPATH_BOUNDS);
            if (node != null)
            {
                Bounds = XmlHelper.GetBoundsRectangle(node.InnerText);
            }

            node = pSkinNode.SelectSingleNode(XPATH_TEXT_MEDIA_CREATOR);
            if (node != null)
            {
                Text textView = new Text();
                textView.ApplySkin(node, pSkinPath);
                MediaManager.MediaChanged += (sender, e) => 
                    {
                        textView.TextString = e.MediaDetail.Artist;
                    };
                AddViewControl(textView);
                textView.StartRender();
            }

            node = pSkinNode.SelectSingleNode(XPATH_TEXT_MEDIA_TITLE);
            if (node != null)
            {
                Text textView = new Text();
                textView.ApplySkin(node, pSkinPath);
                MediaManager.MediaChanged += (sender, e) =>
                {
                    textView.TextString = e.MediaDetail.Title;
                };
                AddViewControl(textView);
                textView.StartRender();
            }

            node = pSkinNode.SelectSingleNode(XPATH_BACKGROUND_IMAGE);
            if (node != null)
            {
                _backgroundBitmapData = new Direct2D.BitmapData(
                    System.IO.Path.Combine(pSkinPath, node.InnerText));
            }

            AnimationPath path = this.CreateAnimationPath();
            foreach (XmlNode pointNode in pSkinNode.SelectNodes(XPATH_ANIMATION_POINT))
            {
                AnimationPathPoint point = XmlHelper.GetAnimationPathPoint(pointNode.InnerText);
                if (path.AnimationPointCount == 0)
                    SetLocation(point.Location);
                path.AddAnimationPoint(point);
            }
            this.SetAnimation(-1);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);

            if (_backgroundImage == null
                && _backgroundBitmapData.Data != null)
            {
                _backgroundImage = Direct2D.GetBitmap(_backgroundBitmapData, pRenderTarget.Renderer);
            }
            if (_backgroundImage != null)
            {
                pRenderTarget.DrawBitmap(_backgroundImage, new System.Drawing.RectangleF(0, 0, _backgroundBitmapData.Width, _backgroundBitmapData.Height));
            }
        }
    }
}
