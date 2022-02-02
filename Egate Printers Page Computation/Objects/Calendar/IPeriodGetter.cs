using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation.Objects.Calendar
{
    public interface IPeriodGetter
    {
        IEnumerable<DateTime> GetPeriodDates(int year, int month);
    }
}
