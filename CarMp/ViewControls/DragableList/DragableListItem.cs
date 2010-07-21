using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMp.ViewControls
{
    public abstract class DragableListItem : D2DViewControl, IDisposable
    {
        private const int SELECTION_BORDER_PADDING = 1;

        private LinearGradientBrush SelectionGradient;

        // Private members
        private int m_index;
        private String m_txtLabelString;
        private Font m_txtLabelFont;
        private Boolean m_selected;
        private System.Drawing.Bitmap m_canvas;
        private bool m_buffered;
        private Size m_size;

        // Rectangle used to create selection square
        private RectF m_selectionRectangle;

        // Constructors

        public DragableListItem()
        {
        }

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(sender, e);

            m_selectionRectangle = new RectF(
                SELECTION_BORDER_PADDING,
                SELECTION_BORDER_PADDING,
                Width - (SELECTION_BORDER_PADDING * 2),
                Height - (SELECTION_BORDER_PADDING * 2)
                );
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

        /// <summary>
        /// Send Touch events to Parent (List)
        /// </summary>
        /// <param name="pTouch"></param>
        public override void SendTouch(Reactive.Touch.Touch pTouch)
        {
            if (Parent != null
                && Parent is DragableList)
                Parent.SendTouch(pTouch);

            base.SendTouch(pTouch);
        }
        
        /// <summary>
        /// Override this
        /// </summary>
        /// <param name="pCanvas"></param>
        protected override void OnRender(CarMp.Direct2D.RenderTargetWrapper pRenderer) 
        {
            if (m_selected)
            {
                if (SelectionGradient == null)
                    SelectionGradient = pRenderer.Renderer.CreateLinearGradientBrush(
                            new LinearGradientBrushProperties()
                            {
                                StartPoint = new Point2F(0, 0),
                                EndPoint = new Point2F(0, _bounds.Height)
                            },
                            pRenderer.Renderer.CreateGradientStopCollection(new GradientStop[] {
                                new GradientStop
                                    {
                                        Color = new ColorF(Colors.Gray, 1),
                                        Position = 0
                                    }
                                    ,
                                new GradientStop
                                    {
                                        Color = new ColorF(Colors.Blue, 1),
                                        Position = 1
                                    }
                                },
                                Gamma.Gamma_10,
                                ExtendMode.Clamp
                        ));

                pRenderer.DrawRectangle(SelectionGradient, m_selectionRectangle, 2F);
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
