using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// Local NameSpaces
using CarMp.Forms;
using System.Drawing;

namespace CarMp
{
    public class FormHost
    {
        public const string DIGITAL_AUDIO = "DigitalAudio";
        public const string HOME = "Home";
        public const string NAVIGATION = "Navigation";
        public const string OPTIONS = "Options";

        private Dictionary<string, Form> _loadedForms;
        private Form _currentForm;

        private FormDebug _debugForm;

        public FormHost()
        {
            if (SessionSettings.Debug)
            {
                OpenDebugForm();
            }
            _loadedForms = new Dictionary<string, Form>();
        }

        public Form OpenForm(string pFormName, bool pShowForm)
        {
            if (_currentForm != null)
            {
                _currentForm.Hide();
            }

            if(_loadedForms.ContainsKey(pFormName))
                _currentForm = _loadedForms[pFormName];
            else
            {
                // Create one.
                _currentForm = CreateForm(pFormName);
            }

            if(pShowForm)
                _currentForm.Show();

            return _currentForm;
        }

        private Form CreateForm(string pFormName)
        {
            Form newForm = null;

            switch (pFormName)
            {
                case DIGITAL_AUDIO:
                    newForm = new FormDigitalAudio();
                    break;
                case HOME:
                    newForm = new FormHome();
                    break;
                case NAVIGATION:
                    newForm = new FormNavigation();
                    break;
                case OPTIONS:
                    newForm = new FormOptions();
                    break;
                default:
                    throw new Exception("Attempt to create an invalid form: " + pFormName);
            }
            newForm.StartPosition = FormStartPosition.Manual;
            newForm.Location = new Point(0, 0);
            
            _loadedForms.Add(pFormName, newForm);
            return newForm;
        }

        private void OpenDebugForm()
        {
            _debugForm = new FormDebug();

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
