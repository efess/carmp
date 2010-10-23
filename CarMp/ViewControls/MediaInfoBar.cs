using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP.ViewControls
{
    public class MediaInfoBar : AnimationContainer, ISkinable
    {
        private const string XPATH_TEXT_MEDIA_TITLE = "MediaTitle";
        private const string XPATH_MEDIA_ART = "MediaArt";
        private const string XPATH_ANIMATION_POINT = "AnimationPath/*";

        private Direct2D.BitmapData _backgroundBitmapData;
        private D2DBitmap _backgroundImage = null;

        private D2DBitmap _artImage = null;
        private Direct2D.BitmapData _artBitmapData;

        private RectF _artBounds;

        public void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            Clear();

            var node = pSkinNode.SelectSingleNode(XPATH_TEXT_MEDIA_TITLE);
            if (node != null)
            {
                Text textView = new Text();
                textView.ApplySkin(node, pSkinPath);
                AppMain.MediaManager.MediaChanged += (sender, e) =>
                {
                    textView.TextString = e.MediaDetail.DisplayName; ;
                };
                AddViewControl(textView);
                textView.StartRender();
            }

            SkinningHelper.XmlRectangleFEntry(XPATH_MEDIA_ART, pSkinNode, ref _artBounds);
            if (!_artBounds.IsEmpty())
            {
                AppMain.MediaManager.MediaChanged += (sender, e) =>
                {
                    SetArt(AppMain.MediaManager.GetCurrentSmallAlbumArt());
                };
            }

            //node = pSkinNode.SelectSingleNode(XPATH_BACKGROUND_IMAGE);
            //if (node != null)
            //{
            //    _artImage = null;
            //    _backgroundImage = null;
            //    _backgroundBitmapData = new Direct2D.BitmapData(
            //        System.IO.Path.Combine(pSkinPath, node.InnerText));
            //}

            //ClearAnimations();
            //AnimationPath path = this.CreateAnimationPath();
            //foreach (XmlNode pointNode in pSkinNode.SelectNodes(XPATH_ANIMATION_POINT))
            //{
            //    AnimationPathPoint point = XmlHelper.GetAnimationPathPoint(pointNode.InnerText);
            //    if (path.AnimationPointCount == 0)
            //        SetLocation(point.Location);
            //    path.AddAnimationPoint(point);
            //}
            //this.SetAnimation(-1);
        }

        public void SetArt(string pPath)
        {
            if (!string.IsNullOrEmpty(pPath))
                _artBitmapData = new Direct2D.BitmapData(pPath);
            else _artBitmapData = new Direct2D.BitmapData(); ;
            _artImage = null;
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);


            if (_backgroundImage == null
                && _backgroundBitmapData.Data != null)
            {
                _backgroundImage = D2DStatic.GetBitmap(_backgroundBitmapData, pRenderTarget.Renderer);
            }
            if (_backgroundImage != null)
            {
                pRenderTarget.DrawBitmap(_backgroundImage, new RectF(0, 0, _backgroundBitmapData.Width, _backgroundBitmapData.Height));
            }

            if (!_artBounds.IsEmpty()
                && _artImage == null
                && _artBitmapData.Data != null)
            {
                _artImage = D2DStatic.GetBitmap(_artBitmapData, pRenderTarget.Renderer);
            }
            if (_artImage != null)
            {
                pRenderTarget.DrawBitmap(_artImage, _artBounds);
            }
        }
    }
}
