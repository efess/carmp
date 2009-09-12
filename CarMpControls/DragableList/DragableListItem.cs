using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CarMpControls
{
    public abstract class DragableListItem : IDisposable
    {
        private const int SELECTION_BORDER_PADDING = 1;

        // Private members
        private int m_index;
        private String m_txtLabelString;
        private Font m_txtLabelFont;
        private Boolean m_selected;
        private Bitmap m_canvas;
        private bool m_buffered;
        private Size m_size;

        // Rectangle used to create selection square
        private Rectangle m_selectionRectangle;

        // Constructors

        public DragableListItem()
        {
        }

        // Accessors
        public Size ClientSize
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
                m_selectionRectangle = new Rectangle(
                    SELECTION_BORDER_PADDING,
                    SELECTION_BORDER_PADDING,
                    value.Width - (SELECTION_BORDER_PADDING * 2),
                    value.Height - (SELECTION_BORDER_PADDING * 2)
                    );
            }
        }

        internal Boolean Buffered
        {
            get {return m_buffered;}
            set
            {
                if (value == m_buffered)
                    return;

                if (value)
                    this.GenerateCanvas();
                else
                    this.ClearCanvas();
            }
        }

        internal int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        internal Boolean Selected
        {
            get { return m_selected; }
            set 
            {
                if (m_selected == value)
                    return;

                m_selected = value; 
                GenerateCanvas();
            }
        }

        // Methods

        internal Bitmap GetCanvas()
        {
            if (!m_buffered)
                GenerateCanvas();

            return m_canvas;
        }

        private void ClearCanvas()
        {
            if (m_canvas != null)
            {
                m_canvas.Dispose();
                m_canvas = null;
            }
            m_buffered = false;
        }

        internal virtual void DrawSelection(Graphics pCanvas)
        {
            Pen SelectionPen = new Pen(Color.FromArgb(255, Color.Blue));
            pCanvas.DrawRectangle(SelectionPen, m_selectionRectangle);
            SelectionPen.Dispose();
        }

        /// <summary>
        /// Override this
        /// </summary>
        /// <param name="pCanvas"></param>
        public virtual void DrawOnCanvas(Graphics pCanvas)
        {
        }

        private void GenerateCanvas()
        {
            m_canvas = new Bitmap(m_size.Width, m_size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(m_canvas);

            if(m_selected)
                DrawSelection(g);
            
            DrawOnCanvas(g);

            g.Dispose();
            m_buffered = true;
        }

        #region IDisposable Members
        
        /// <summary>
        /// If you override this, you should still call base.Dispose() to make sure base objects are disposed as well.
        /// </summary>
        public virtual void Dispose()
        {
            if (m_txtLabelFont != null)
                m_txtLabelFont.Dispose();

            if (m_canvas != null)
                m_canvas.Dispose();
        }

        #endregion
    }
}
