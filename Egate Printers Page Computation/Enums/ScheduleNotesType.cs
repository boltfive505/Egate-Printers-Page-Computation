using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ScheduleNotesType
    {
        [Description("")]
        None,
        [Description("For Follow-up Next Week")]
        ForFollowupNextWeek,
        [Description("For Follow-up Tomorrow")]
        ForFollowupTomorrow,
        [Description("For Pick-up of Check")]
        ForPickupOfCheck,
        [Description("For Review of Sales Invoice")]
        ForReviewOfSalesInvoice,
        [Description("For Clarification of Billing")]
        ForClarificationOfBilling,
        [Description("For Signature")]
        ForSignature,
        [Description("Issue On Collection")]
        IssueOnCollection,
        [Description("Not Scheduled")]
        NotScheduled,
        [Description("Others")]
        Others
    }
}
