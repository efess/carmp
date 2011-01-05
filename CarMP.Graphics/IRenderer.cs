using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics
{
    public interface IRenderer
    {
        /// <summary>
        /// Resize rendering area
        /// </summary>
        /// <param name="pSize"></param>
        void Resize(Size pSize);
        void BeginDraw();
        void EndDraw();

    }
}
