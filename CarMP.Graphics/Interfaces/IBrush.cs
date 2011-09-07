using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics.Interfaces
{
    /// <summary>
    /// Implementations for brush - determines how to color a primative
    /// </summary>
    public interface IBrush
    {
        Color Color { get; set; }
    }
}
