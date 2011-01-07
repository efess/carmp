using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics.Interfaces
{
    internal interface IColorResolver
    {
        Color GetColor(string pColorName);
    }
}
