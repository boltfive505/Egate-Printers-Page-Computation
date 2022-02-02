using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation.Objects.Calendar
{
    public class PeriodCalendarDisplayCollection<T> : INotifyPropertyChanged, CustomMonthlyCalendar.IMonthlyCalendarDayItem where T : IPeriodGetter
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public DateTime Day { get; set; }
        public List<PeriodCalendarDisplay<T>> Items { get; set; } = new List<PeriodCalendarDisplay<T>>();
    }
}
