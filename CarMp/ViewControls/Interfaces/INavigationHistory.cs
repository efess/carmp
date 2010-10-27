using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.ViewControls.Interfaces
{
    public interface INavigationHistory
    {
        Func<IEnumerable<KeyValuePair<int, string>>> GetHistorySource { get; set; }
        event Action<int> HistoryClick;
    }
}
