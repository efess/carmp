using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMp.ViewControls
{
    public class MediaControlBar : AnimationContainer
    {
        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_BUTTONS = "Buttons/*";
        private const string XPATH_PROGRESS_BAR = "ThermometerProgressBar";
        private const string XPATH_ANIMATION_POINT = "AnimationPath/*";
        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";
        private const string XPATH_POSITION_TEXT = "PositionText";

        private const string XPATH_NODE_PLAY = "Play";
        private const string XPATH_NODE_STOP = "Stop";
        private const string XPATH_NODE_PAUSE = "Pause";
        private const string XPATH_NODE_PREVIOUS = "Previous";
        private const string XPATH_NODE_NEXT = "Next";

        private ThermometerProgressBar _progressBar;
        private Direct2D.BitmapData _backgroundBitmapData;
        private D2DBitmap _backgroundImage = null;

        public void ApplySkin(XmlNode pSkinViewNode, string pSkinPath)
        {
            Clear();
            
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

            node = pSkinViewNode.SelectSingleNode(XPATH_POSITION_TEXT);
            if (node != null)
            {
                Text text = new Text();
                text.ApplySkin(node, pSkinPath);
                AppMain.MediaManager.MediaProgressChanged += new MediaProgressChangedHandler(
                    (sender, eventArgs) =>
                    {
                        text.TextString = ((eventArgs.MediaPosition / 1000) / 60) 
                            + ":" 
                            + ((eventArgs.MediaPosition / 1000) % 60).ToString().PadLeft(2,'0');
                    });
                text.StartRender();
                AddViewControl(text);
            }

            node = pSkinViewNode.SelectSingleNode(XPATH_PROGRESS_BAR);
            if (node != null)
            {
                _progressBar = new ThermometerProgressBar();
                _progressBar.ApplySkin(node, pSkinPath);
                _progressBar.ScrollChanged += (sender, e) =>
                {
                    AppMain.MediaManager.SetCurrentPos(Convert.ToInt32(_progressBar.Value));
                };

                AppMain.MediaManager.MediaProgressChanged += new MediaProgressChangedHandler(
                    (sender, eventArgs) => 
                        {
                            _progressBar.Value = eventArgs.MediaPosition;
                        });

                AppMain.MediaManager.MediaChanged += new MediaChangedHandler(
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
                        button.Click += (sender, e) => AppMain.MediaManager.StartPlayback();
                        button.ButtonString = XPATH_NODE_PLAY;
                        break;
                    case XPATH_NODE_STOP:
                        button.Click += (sender, e) => AppMain.MediaManager.StopPlayback();
                        button.ButtonString = XPATH_NODE_STOP;
                        break;
                    case XPATH_NODE_PAUSE:
                        button.Click += (sender, e) => AppMain.MediaManager.PausePlayback();
                        break;
                    case XPATH_NODE_NEXT:
                        button.Click += (sender, e) => AppMain.MediaManager.MediaNext();
                        break;
                    case XPATH_NODE_PREVIOUS:
                        button.Click += (sender, e) => AppMain.MediaManager.MediaPrevious();
                        break;
                    //case XPATH_NODE_PREVIOUS:
                    //    button.Click += new EventHandler((sender, e) => AppMain.MediaManager.());
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
                pRenderTarget.DrawBitmap(_backgroundImage, new RectF(0, 0, _backgroundBitmapData.Width, _backgroundBitmapData.Height));
            }
        }
    }
}
