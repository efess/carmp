using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;
using System.Xml;

namespace CarMP.ViewControls
{
    public abstract class DragableListItem : ViewControlCommonBase, IDisposable, ISkinable
    {
        private const int SELECTION_BORDER_PADDING = 1;
        private const int BACKGROUND_BOUNDS_PADDING = 2;

        private LinearGradientBrush SelectionGradient;

        // Private members
        private int m_index;
        private String m_txtLabelString;
        private Boolean m_selected;
        private System.Drawing.Bitmap m_canvas;
        private bool m_buffered;
        private Brush _backgroundBrush;

        public object Tag { get; set; }

        // Rectangle used to create selection square
        private RectF m_selectionRectangle;

        private RectF _backgroundRect;
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
                Width - SELECTION_BORDER_PADDING,
                Height - SELECTION_BORDER_PADDING
                );

        }

        public virtual void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            base.ApplySkin(pXmlNode, pSkinPath);
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
        public override void SendUpdate(Reactive.ReactiveUpdate pReactiveUpdate)
        {
            if (Parent != null
                && Parent is DragableList)
                Parent.SendUpdate(pReactiveUpdate);

            base.SendUpdate(pReactiveUpdate);
        }
        
        /// <summary>
        /// Override this
        /// </summary>
        /// <param name="pCanvas"></param>
        protected override void OnRender(CarMP.Direct2D.RenderTargetWrapper pRenderer) 
        {
            if(_backgroundBrush == null)
            {
                _backgroundBrush = pRenderer.Renderer.CreateSolidColorBrush(new ColorF(Colors.Black, .3f));
            }
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
            pRenderer.FillRectangle(_backgroundBrush, _backgroundRect);
        }



        #region IDisposable Members

        /// <summary>
        /// If you override this, you should still call base.Dispose() to make sure base objects are disposed as well.
        /// </summary>
        public virtual void Dispose()
        {
            if (_backgroundBrush != null)
                _backgroundBrush.Dispose();

            if (SelectionGradient != null)
                SelectionGradient.Dispose();
        }

        #endregion
    }
}
