using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CarMp.Forms
{
    public partial class FormHost : Form
    {
        public const string DIGITAL_AUDIO = "DigitalAudio";
        public const string HOME = "Home";
        public const string NAVIGATION = "Navigation";
        public const string OPTIONS = "Options";

        private Dictionary<string, ContentBase> _loadedContent;
        private ContentBase _currentContent;

        private FormDebug _debugForm;

        public FormHost()
        {
            if (SessionSettings.Debug)
            {
                OpenDebugForm();
            }

            _loadedContent = new Dictionary<string, ContentBase>();
            InitializeComponent();
            this.Size = SessionSettings.ScreenResolution;
            this.Location = SessionSettings.WindowLocation;
        }

        public ContentBase OpenContent(string pContentName, bool pShowForm)
        {
            if (_currentContent != null)
            {
                pnlContent.Controls.Clear();
                //_currentContent.Hide();
            }

            if(_loadedContent.ContainsKey(pContentName))
                _currentContent = _loadedContent[pContentName];
            else
            {
                // Create one.
                _currentContent = CreateContent(pContentName);
                _loadedContent.Add(pContentName, _currentContent);
            }

            pnlContent.Controls.Add(_currentContent);
            _currentContent.Dock = DockStyle.Fill;
                //_currentContent.Show();
            

            return _currentContent;
        }

        private ContentBase CreateContent(string pContentName)
        {
            ContentBase newContent = null;

            switch (pContentName)
            {
                case DIGITAL_AUDIO:
                    newContent = new ContentDigitalAudio();
                    break;
                case HOME:
                    newContent = new ContentHome();
                    break;
                case NAVIGATION:
                    newContent = new ContentNavigation();
                    break;
                case OPTIONS:
                    newContent = new ContentOptions();
                    break;
                default:
                    throw new Exception("Attempt to create an invalid form: " + pContentName);
            }

            return newContent;
        }

        private void OpenDebugForm()
        {
            _debugForm = new FormDebug();
            
            DebugHandler.De += new DebugHandler.DebugException(ex => _debugForm.WriteException(ex));
            DebugHandler.Ds += new DebugHandler.DebugString(str => _debugForm.WriteText(str));

            _debugForm.Location =
                new Point(
                    Screen.PrimaryScreen.Bounds.Width - _debugForm.Width - 20,
                    Screen.PrimaryScreen.Bounds.Height - _debugForm.Height - 40
                    );

            _debugForm.StartPosition = FormStartPosition.Manual;
            _debugForm.Show();
        }
    }
}
