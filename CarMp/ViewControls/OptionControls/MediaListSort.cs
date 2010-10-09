using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMP.ViewControls.OptionControls
{
    public class MediaListSort : Container, IOptionControl
    {
        private const string OPTION_NAME = "Media List Sort";
        private const string OPTION_ELEMENT = "MediaListSort";

        public string OptionName { get { return OPTION_NAME; } }
        public string OptionElement { get { return OPTION_ELEMENT; } }

        public MediaListSort()
        {
            var option = new SingleOptionRadio();
            option.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(5, 5, 200, 30);
            AddViewControl(option);
            option.TextString = "Track";
            option.StartRender();
            option.InputLeave += () => SetFormat(MediaSort.Track);

            option = new SingleOptionRadio();
            option.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(5, 40, 200, 65);
            AddViewControl(option);
            option.TextString = "FileName";
            option.StartRender();
            option.InputLeave += () => SetFormat(MediaSort.FileName);

            option = new SingleOptionRadio();
            option.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(5, 75, 200, 100);
            AddViewControl(option);
            option.TextString = "Title";
            option.InputLeave += () => SetFormat(MediaSort.Title);
            option.StartRender();
        }

        protected void SetFormat(MediaSort pMediaSort)
        {
            AppMain.Settings.SortMedia = pMediaSort;
            AppMain.Settings.SaveXml();
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
        }
    }
}
