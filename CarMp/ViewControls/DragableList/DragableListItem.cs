using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using SlimDX;
using SlimDX.Direct2D;
using SlimDX.Windows;

namespace CarMp.ViewControls
{
    public abstract class DragableListItem : IDisposable
    {
        private const int SELECTION_BORDER_PADDING = 1;

        private SlimDX.Direct2D.LinearGradientBrush SelectionGradient;

        // Private members
        private int m_index;
        private String m_txtLabelString;
        private Font m_txtLabelFont;
        private Boolean m_selected;
        private System.Drawing.Bitmap m_canvas;
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
            }
        }

        // Methods

        internal System.Drawing.Bitmap GetCanvas()
        {
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
        internal virtual void DrawItem(Direct2D.RenderTargetWrapper pRenderer, RectangleF pRectangle) 
        {
            if (m_selected)
            {
                if (SelectionGradient == null)
                    SelectionGradient = new SlimDX.Direct2D.LinearGradientBrush(pRenderer.Renderer,
                        new GradientStopCollection(pRenderer.Renderer, new GradientStop[] {
                        new GradientStop
                            {
                                Color = Color.Gray,
                                Position = 0
                            }
                            ,
                        new GradientStop
                            {
                                Color = Color.Blue,
                                Position = 1
                            }
                        }),
                            new LinearGradientBrushProperties()
                            {
                                StartPoint = new PointF(pRectangle.Top, 0),
                                EndPoint = new PointF(0, pRectangle.Bottom)
                            }
                        );
                ////
                //// GOTTA IMPLEMENT ONRENDER here so the child list items can
                //// have its bounds corrected by the parent!
                ////
                //// DrawItem is *old*!!

                pRenderer.DrawRectangle(SelectionGradient, pRectangle, 2F);
            }
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
