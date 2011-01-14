using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Interfaces;

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
            MediaSort currentSort = AppMain.Settings.SortMedia;

            var option = new SingleOptionRadio();
            option.Bounds = new Rectangle(5, 5, 195, 25);
            AddViewControl(option);
            option.TextString = "Track";
            option.Checked = currentSort == MediaSort.Track;
            option.StartRender();
            option.InputLeave += () => SetFormat(MediaSort.Track);

            option = new SingleOptionRadio();
            option.Bounds = new Rectangle(5, 40, 195, 25);
            AddViewControl(option);
            option.TextString = "FileName";
            option.StartRender();
            option.Checked = currentSort == MediaSort.FileName;
            option.InputLeave += () => SetFormat(MediaSort.FileName);

            option = new SingleOptionRadio();
            option.Bounds = new Rectangle(5, 75, 195, 25);
            AddViewControl(option);
            option.TextString = "Title";
            option.Checked = currentSort == MediaSort.Title;
            option.InputLeave += () => SetFormat(MediaSort.Title);
            option.StartRender();
        }

        protected void SetFormat(MediaSort pMediaSort)
        {
            AppMain.Settings.SortMedia = pMediaSort;
            AppMain.Settings.SaveXml();
        }

        protected override void OnRender(IRenderer pRenderer)
        {
        }
    }
}
