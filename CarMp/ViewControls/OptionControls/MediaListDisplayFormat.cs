using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.ViewControls.OptionControls
{
    public class MediaListDisplayFormat : D2DViewControl, IOptionControl
    {
        private const string OPTION_NAME = "Media Display Format";
        private const string OPTION_ELEMENT = "MediaDisplayFormat";

        private const string TRACK_ARTIST_TITLE = "%track% - %artist% - %title%";
        private const string ARTIST_TITLE = "%artist% - %title%";
        private const string TRACK_TITLE = "%track% = %title%";
        private const string TITLE = "%title%";
        private const string FILENAME = "%filename%";

        public string OptionName { get { return OPTION_NAME; } }
        public string OptionElement { get { return OPTION_ELEMENT; } }

        // NOTE: Need to store in "ItemName" in MediaGroupItem table as separate key/values
        public MediaListDisplayFormat()
        {
            string existingTemplate = AppMain.Settings.DisplayFormat.FormatTemplate;
            var option = new SingleOptionRadio();
            option.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(5, 5, 250, 30);
            AddViewControl(option);
            option.TextString = "Track - Artist - Title";
            option.Checked = existingTemplate == TRACK_ARTIST_TITLE;
            option.InputLeave += () => { SetFormat(TRACK_ARTIST_TITLE); };
            
            option.StartRender();

            option = new SingleOptionRadio();
            option.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(5, 40, 250, 65);
            AddViewControl(option);
            option.TextString = "Artist - Title";
            option.Checked = existingTemplate == ARTIST_TITLE;
            option.InputLeave += () => { SetFormat(ARTIST_TITLE); };
            option.StartRender();

            option = new SingleOptionRadio();
            option.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(5, 75, 250, 100);
            AddViewControl(option);
            option.TextString = "Track - Title";
            option.Checked = existingTemplate == TRACK_TITLE;
            option.InputLeave += () => { SetFormat(TRACK_TITLE); };
            option.StartRender();

            option = new SingleOptionRadio();
            option.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(260, 5, 460, 30);
            AddViewControl(option);
            option.Checked = existingTemplate == TITLE;
            option.TextString = "Title";
            option.StartRender();
            option.InputLeave += () => { SetFormat(TITLE); };

            option = new SingleOptionRadio();
            option.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(260, 40, 460, 65);
            AddViewControl(option);
            option.TextString = "FileName";
            option.Checked = existingTemplate == FILENAME;
            option.InputLeave += () => { SetFormat(FILENAME); };
            option.StartRender();


            var chkoption = new CheckBox();
            chkoption.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(5, 110, 250, 135);
            AddViewControl(chkoption);
            chkoption.Checked = AppMain.Settings.DisplayFormat.ReplacePercentTwenty;
            chkoption.TextString = "Replaced %20 With Space";
            chkoption.InputLeave += () => { var thisOption = chkoption; TogglePercentTwentyReplace(thisOption.Checked); };
            chkoption.StartRender();

            chkoption = new CheckBox();
            chkoption.Bounds = new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.RectF(5, 145, 250, 170);
            AddViewControl(chkoption);
            chkoption.Checked = AppMain.Settings.DisplayFormat.ReplaceUnderscores;
            chkoption.TextString = "Replaced Underscores With Space";
            chkoption.InputLeave += () => { var thisOption = chkoption;ToggleUnderscoreReplace(thisOption.Checked); };
            chkoption.StartRender();
        }

        protected void ToggleUnderscoreReplace(bool doIt)
        {
            AppMain.Settings.DisplayFormat.ReplaceUnderscores = doIt;
            AppMain.Settings.SaveXml();
        }

        protected void TogglePercentTwentyReplace(bool doIt)
        {
            AppMain.Settings.DisplayFormat.ReplacePercentTwenty = doIt;
            AppMain.Settings.SaveXml();
        }

        protected void SetFormat(string pTemplateFormat)
        {
            AppMain.Settings.DisplayFormat.FormatTemplate = pTemplateFormat;
            AppMain.Settings.SaveXml();
        }


        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            
        }
    }
}
