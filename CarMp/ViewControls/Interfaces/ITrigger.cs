using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.ViewControls.Interfaces
{
    public interface ITrigger
    {
        Action<object> Trigger { get; set; }
    }
}
