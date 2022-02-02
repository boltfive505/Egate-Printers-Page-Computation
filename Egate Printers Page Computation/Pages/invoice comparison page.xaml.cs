using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Egate_Printers_Page_Computation.Pages
{
    /// <summary>
    /// Interaction logic for collection_page.xaml
    /// </summary>
    public partial class invoice_comparison_page : Page
    {
        public invoice_comparison_page()
        {
            InitializeComponent();
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DoComparisonMarking();
        }

        private void InvoiceList_FilterChanged()
        {
            DoComparisonMarking();
        }

        private void DoComparisonMarking()
        {
            //clear markings
            comparisonLeft.FilteredInvoiceList.ForEach(i => i.ComparisonMark = false);
            comparisonRight.FilteredInvoiceList.ForEach(i => i.ComparisonMark = false);
            //get comparisons
            //left
            foreach (var left in comparisonLeft.FilteredInvoiceList)
            {
                left.ComparisonMark = !comparisonRight.FilteredInvoiceList.Any(i => i.Company.Id == left.Company.Id && CompareNotes(i.Notes, left.Notes));
            }
            //right
            foreach (var right in comparisonRight.FilteredInvoiceList)
            {
                right.ComparisonMark = !comparisonLeft.FilteredInvoiceList.Any(i => i.Company.Id == right.Company.Id && CompareNotes(i.Notes, right.Notes));
            }
        }

        private bool CompareNotes(string notes1, string notes2)
        {
            if (notes1 == null) notes1 = string.Empty;
            if (notes2 == null) notes2 = string.Empty;
            return notes1.Trim().ToLower() == notes2.Trim().ToLower();
        }
    }
}
