using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace CarMP.Graphics.Implementation.OpenTk
{
    public abstract class OTKDrawableObjectBase
    {
        protected abstract void DrawObject();

        private bool _enableBlending;
        private byte _blendingAlpha;

        internal void Draw()
        {
            BeginDraw();

            DrawObject();

            EndDraw();
        }

        internal void SetBlending(float pAlpha)
        {
            if (pAlpha > 1)
                _blendingAlpha = 0xFF;
            if (pAlpha < 0)
                _blendingAlpha = 0x00;

            _blendingAlpha = (byte)(255 * pAlpha);
        }

        private void BeginDraw()
        {
            if (_enableBlending)
            {
                GL.Enable(EnableCap.Blend);
                GL.Color4((byte)0xFF, (byte)0xFF, (byte)0xFF, _blendingAlpha);
            }
        }

        private void EndDraw()
        {
            if (_enableBlending)
            {
                GL.Disable(EnableCap.Blend);

                // Set white color
                GL.Color4(0xFF, 0xFF, 0xFF, 0xFF);
            }
        }
    }
}
