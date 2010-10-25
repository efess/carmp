using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.ViewControls.Interfaces
{
    public interface IText
    {
        string TextString { get; set; }
        TextStyle TextStyle { get; set; }
    }
}
