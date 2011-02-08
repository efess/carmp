using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CarMP.Graphics.Geometry;
using System.Xml;
using CarMP.Graphics.Interfaces;
using CarMP.Helpers;
using CarMP.Graphics;

namespace CarMP.ViewControls
{
    public abstract class DragableListItem : ViewControlCommonBase, IDisposable
    {
        private const int SELECTION_BORDER_PADDING = 1;
        private const int BACKGROUND_BOUNDS_PADDING = 2;

        private IBrush _selectionBoxBrush;

        // Private members
        private int _index;
        private String _txtLabelString;
        private Boolean _selected;
        private System.Drawing.Bitmap m_canvas;
        private bool _buffered;

        public object Tag { get; set; }

        // Rectangle used to create selection square
        private Rectangle _selectionRectangle;

        // Constructors

        public DragableListItem()
        {
        }

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(sender, e);

            _selectionRectangle = new Rectangle(
                SELECTION_BORDER_PADDING,
                SELECTION_BORDER_PADDING,
                Width,
                Height
                );

        }

        public virtual void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            base.ApplySkin(pXmlNode, pSkinPath);
        }

        internal Boolean Buffered
        {
            get {return _buffered;}
            set
            {

                if (value == _buffered)
                    return;
            }
        }

        internal int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public Boolean Selected
        {
            get { return _selected; }
            set 
            {
                if (_selected == value)
                    return;

                _selected = value; 
            }
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
        protected override void OnRender(IRenderer pRenderer) 
        {
            if (_selected)
            {
                if (_selectionBoxBrush == null)
                    _selectionBoxBrush = pRenderer.CreateBrush(Color.RoyalBlue);
                // TODO: Gradients?

                //if (SelectionGradient == null)
                //    SelectionGradient = pRenderer.Renderer.CreateLinearGradientBrush(
                //            new LinearGradientBrushProperties()
                //            {
                //                StartPoint = new Point(0, 0),
                //                EndPoint = new Point(0, _bounds.Height)
                //            },
                //            pRenderer.Renderer.CreateGradientStopCollection(new GradientStop[] {
                //                new GradientStop
                //                    {
                //                        Color = new ColorF(Colors.Gray, 1),
                //                        Position = 0
                //                    }
                //                    ,
                //                new GradientStop
                //                    {
                //                        Color = new ColorF(Colors.Blue, 1),
                //                        Position = 1
                //                    }
                //                },
                //                Gamma.Gamma_10,
                //                ExtendMode.Clamp
                //        ));

                pRenderer.DrawRectangle(_selectionBoxBrush, _selectionRectangle, 2F);
            }
            else
            {
                if (_selectionBoxBrush != null)
                {
                    Helpers.GraphicsHelper.DisposeIfImplementsIDisposable(_selectionBoxBrush);
                    _selectionBoxBrush = null;
                }
            }
           // pRenderer.FillRectangle(_backgroundBrush, _backgroundRect);
        }



        #region IDisposable Members

        /// <summary>
        /// If you override this, you should still call base.Dispose() to make sure base objects are disposed as well.
        /// </summary>
        public virtual void Dispose()
        {
            //if (SelectionGradient != null)
            //    GraphicsHelper.DisposeIfImplementsIDisposable(_selectionBoxBrush);
        }

        #endregion
    }
}
