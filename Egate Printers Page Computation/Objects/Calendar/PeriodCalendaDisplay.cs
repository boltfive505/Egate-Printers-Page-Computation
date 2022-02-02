using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation.Objects.Calendar
{
    public class PeriodCalendarDisplay<T> : INotifyPropertyChanged where T : IPeriodGetter
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public DateTime PeriodDate { get; set; }
        public T Item { get; set; }
    }
}
