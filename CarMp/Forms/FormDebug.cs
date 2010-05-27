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
    public partial class FormDebug : Form
    {
        delegate void RefreshTextCallback();
        private List<String> m_debugBuffer;
        private int m_maxDisplayLines;

        public FormDebug()
        {
            InitializeComponent();
            m_maxDisplayLines = 1000;
            m_debugBuffer = new List<String>();
        }

        public void WriteText(String text)
        {            
            m_debugBuffer.Add(DateTime.Now.ToString() + "\tDBG> " + text);
            this.RefreshText();
        }

        public void WriteException(Exception e)
        {
            m_debugBuffer.Add(DateTime.Now.ToString() + "\tEXCEPTION DBG> " + e.Message + " StackTrace: " + e.StackTrace);
            this.RefreshText();
        }
        
        private void RefreshText()
        {
            if (txtDebug.InvokeRequired)
            {
                this.Invoke(new RefreshTextCallback(RefreshText));
            }
            else
            {
                StringBuilder sb = new StringBuilder("");

                for (int i = (m_debugBuffer.Count > m_maxDisplayLines) ? m_debugBuffer.Count - m_maxDisplayLines : 0;
                    i < m_debugBuffer.Count; i++)
                {
                    sb.AppendLine(m_debugBuffer[i]);
                }

                txtDebug.Text = sb.ToString();

                txtDebug.Select(txtDebug.Text.Length + 1, 2);
                txtDebug.ScrollToCaret();
            }
        }
    }
}
