using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMp.ViewControls
{
    public class MediaQuickBar : AnimationContainer
    {
        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_BUTTONS = "Buttons/*";
        private const string XPATH_ANIMATION_POINT = "AnimationPath/*";
        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";

        private const string XPATH_NODE_PLAY = "Play";
        private const string XPATH_NODE_STOP = "Stop";
        private const string XPATH_NODE_PAUSE = "Pause";
        private const string XPATH_NODE_PREVIOUS = "Previous";
        private const string XPATH_NODE_NEXT = "Next";

        private Direct2D.BitmapData _backgroundBitmapData;
        private SlimDX.Direct2D.Bitmap _backgroundImage = null;

        public void ApplySkin(XmlNode pSkinViewNode, string pSkinPath)
        {
            _viewControls.Clear();
            
            XmlNode node = pSkinViewNode.SelectSingleNode(XPATH_BOUNDS);
            if (node != null)
            {
                Bounds = XmlHelper.GetBoundsRectangle(node.InnerText);
            }
            
            node = pSkinViewNode.SelectSingleNode(XPATH_BACKGROUND_IMAGE);
            if (node != null)
            {
                _backgroundBitmapData = new Direct2D.BitmapData(
                    System.IO.Path.Combine(pSkinPath, node.InnerText));
            }
            

            AnimationPath path = this.CreateAnimationPath();
            foreach (XmlNode pointNode in pSkinViewNode.SelectNodes(XPATH_ANIMATION_POINT))
            {
                path.AddAnimationPoint(XmlHelper.GetAnimationPathPoint(pointNode.InnerText));
            }

            foreach (XmlNode buttonNode in pSkinViewNode.SelectNodes(XPATH_BUTTONS))
            {
                GraphicalButton button = new GraphicalButton();
                button.ApplySkin(buttonNode, pSkinPath);
                AddViewControl(button);
                button.StartRender();

                switch (buttonNode.Name)
                {
                    case XPATH_NODE_PLAY:
                        button.Click += new EventHandler((sender, e) => MediaManager.StartPlayback());
                        button.ButtonString = XPATH_NODE_PLAY;
                        break;
                    case XPATH_NODE_STOP:
                        button.Click += new EventHandler((sender, e) => MediaManager.StopPlayback());
                        button.ButtonString = XPATH_NODE_STOP;
                        break;
                    case XPATH_NODE_PAUSE:
                        button.Click += new EventHandler((sender, e) => MediaManager.PausePlayback());
                        break;
                    //case XPATH_NODE_PREVIOUS:
                    //    button.Click += new EventHandler((sender, e) => MediaManager.());
                    //    break;
                }
            }
            this.SetAnimation(-1);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);

            if (_backgroundImage == null
                && _backgroundBitmapData != null)
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
