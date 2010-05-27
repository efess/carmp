using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp.Views
{
    public class D2DViewFactory
    {
        public const string DIGITAL_AUDIO = "DigitalAudio";
        public const string HOME = "Home";
        public const string NAVIGATION = "Navigation";
        public const string OPTIONS = "Options";

        private System.Drawing.Size _windowSize;

        public D2DViewFactory(System.Drawing.Size pWindowSize)
        {
            _windowSize = pWindowSize;
        }

        public D2DView CreateView(string pViewName)
        {
            D2DView newView = null;

            switch (pViewName)
            {
                case DIGITAL_AUDIO:
                    newView = new MusicView(_windowSize);
                    break;
                case HOME:
                    newView = new HomeView(_windowSize);
                    break;
                //case NAVIGATION:
                //    newView = new ContentNavigation();
                //    break;
                //case OPTIONS:
                //    newView = new ContentOptions();
                //    break;
                default:
                    throw new Exception("Attempt to create an invalid form: " + pViewName);
            }

            return newView;
        }
    }
}
