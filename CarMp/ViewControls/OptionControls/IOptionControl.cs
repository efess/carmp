using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp.ViewControls.OptionControls
{
    public interface IOptionControl
    {
        string OptionName { get; }
        string OptionElement { get; }
    }
}
