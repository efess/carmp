using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Interfaces;

namespace CarMP.ViewControls
{
    public class MediaInfoBar : AnimationContainer, ISkinable
    {
        private const string XPATH_TEXT_MEDIA_TITLE = "MediaTitle";
        private const string XPATH_MEDIA_ART = "MediaArt";
        private const string XPATH_ANIMATION_POINT = "AnimationPath/*";

        private string _backgroundBitmapPath;
        private IImage _backgroundImage = null;

        private IImage _artImage = null;
        private string _artBitmapPath;

        private Rectangle _artBounds;

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

            Helpers.SkinningHelper.XmlRectangleEntry(XPATH_MEDIA_ART, pSkinNode, ref _artBounds);
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
                _artBitmapPath = pPath;
            else _artBitmapPath = null;
            _artImage = null;
        }

        protected override void OnRender(IRenderer pRenderer)
        {
            base.OnRender(pRenderer);


            if (_backgroundImage == null
                && !string.IsNullOrEmpty(_backgroundBitmapPath))
            {
                _backgroundImage = pRenderer.CreateImage(_backgroundBitmapPath);
            }
            if (_backgroundImage != null)
            {
                pRenderer.DrawImage(new Rectangle(0, 0, _backgroundImage.Size.Width, _backgroundImage.Size.Height), _backgroundImage, 1f);
            }

            if (!_artBounds.IsEmpty()
                && _artImage == null
                && _artBitmapPath != null)
            {
                _artImage = pRenderer.CreateImage(_artBitmapPath);
            }
            if (_artImage != null)
            {
                pRenderer.DrawImage(_artBounds, _artImage, 1f);
            }
        }
    }
}
