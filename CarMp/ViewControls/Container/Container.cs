using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Direct2D;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMP.ViewControls
{
    public class Container : ViewControlCommonBase, IDisposable
    {
        private Brush _borderBrush;
        public Container()
        {
            // Defaults
        }

        public override void Dispose()
        {
            if (_borderBrush != null) _borderBrush.Dispose();
            base.Dispose();
        }

        public bool UseBorder { get; set; }

        protected override void OnRender(RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);

            if (UseBorder && _borderBrush == null)
                pRenderTarget.Renderer.CreateSolidColorBrush(new ColorF(Colors.LightGray));

            if(UseBorder)
                pRenderTarget.DrawRectangle(_borderBrush, new RectF(this.Bounds.Left + 1, this.Bounds.Top + 1, this.Bounds.Right - 2, this.Bounds.Bottom - 2), 1);
        }        
    }
}
