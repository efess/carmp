using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMp.ViewControls
{
    public class MediaControlBar : AnimationContainer
    {
        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_BUTTONS = "Buttons/*";
        private const string XPATH_PROGRESS_BAR = "ThermometerProgressBar";
        private const string XPATH_ANIMATION_POINT = "AnimationPath/*";
        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";

        private const string XPATH_NODE_PLAY = "Play";
        private const string XPATH_NODE_STOP = "Stop";
        private const string XPATH_NODE_PAUSE = "Pause";
        private const string XPATH_NODE_PREVIOUS = "Previous";
        private const string XPATH_NODE_NEXT = "Next";

        private ThermometerProgressBar _progressBar;
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

            node = pSkinViewNode.SelectSingleNode(XPATH_PROGRESS_BAR);
            if (node != null)
            {
                _progressBar = new ThermometerProgressBar();
                _progressBar.ApplySkin(node, pSkinPath);
                _progressBar.ScrollChanged += (sender, e) =>
                {
                    MediaManager.SetCurrentPos(_progressBar.Value);
                };

                MediaManager.MediaProgressChanged += new MediaProgressChangedHandler(
                    (sender, eventArgs) => 
                        {
                            _progressBar.Value = eventArgs.MediaPosition;
                        });

                MediaManager.MediaChanged += new MediaChangedHandler(
                    (sender, eventArgs) =>
                        {
                            _progressBar.Value = 0;
                            _progressBar.MaximumValue = eventArgs.MediaDetail.Length;
                        });

                _progressBar.StartRender();
                AddViewControl(_progressBar);
            }           

            AnimationPath path = this.CreateAnimationPath();
            foreach (XmlNode pointNode in pSkinViewNode.SelectNodes(XPATH_ANIMATION_POINT))
            {
                AnimationPathPoint point = XmlHelper.GetAnimationPathPoint(pointNode.InnerText);
                if (path.AnimationPointCount == 0)
                    SetLocation(point.Location);
                path.AddAnimationPoint(point);
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
                        button.Click += (sender, e) => MediaManager.StartPlayback();
                        button.ButtonString = XPATH_NODE_PLAY;
                        break;
                    case XPATH_NODE_STOP:
                        button.Click += (sender, e) => MediaManager.StopPlayback();
                        button.ButtonString = XPATH_NODE_STOP;
                        break;
                    case XPATH_NODE_PAUSE:
                        button.Click += (sender, e) => MediaManager.PausePlayback();
                        break;
                    case XPATH_NODE_NEXT:
                        button.Click += (sender, e) => MediaManager.MediaNext();
                        break;
                    case XPATH_NODE_PREVIOUS:
                        button.Click += (sender, e) => MediaManager.MediaPrevious();
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
