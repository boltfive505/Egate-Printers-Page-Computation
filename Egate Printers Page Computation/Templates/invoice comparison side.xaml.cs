using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Printers_Page_Computation.Objects.CollectionSchedule;
using Egate_Printers_Page_Computation.Helpers;
using System.ComponentModel;

namespace Egate_Printers_Page_Computation.Templates
{
    /// <summary>
    /// Interaction logic for invoice_comparison_side.xaml
    /// </summary>
    public partial class invoice_comparison_side : UserControl
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int FilterInvoiceYear { get; set; }
            public int FilterInvoiceMonth { get; set; }

            public FilterGroup()
            {
                DateTime now = DateTime.Now;
                FilterInvoiceYear = now.Year;
                FilterInvoiceMonth = now.Month;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            //for company
            public int TotalActiveCompany { get; set; }
            //for invoice
            public int TotalInvoiceCompanyCount { get; set; }
            public int TotalInvoiceCount { get; set; }
            public decimal TotalInvoiceAmount { get; set; }
            public int InvoiceMonthOf { get; set; }
        }

        public List<InvoiceViewModel> FilteredInvoiceList { get { return invoiceView == null ? new List<InvoiceViewModel>() : invoiceView.OfType<InvoiceViewModel>().ToList(); } }
        public event Action FilterChanged;

        private ICollectionView invoiceView;
        private List<InvoiceViewModel> invoiceList = new List<InvoiceViewModel>();
        private FilterGroup filtersInvoice;
        private TotalGroup totalsInvoice;

        public invoice_comparison_side()
        {
            InitializeComponent();
            invoiceView = new CollectionViewSource() { Source = invoiceList }.View;
            invoiceView.SortDescriptions.Add(new SortDescription("Company.CompanyName", ListSortDirection.Ascending));
            invoiceView.SortDescriptions.Add(new SortDescription("InvoiceDate", ListSortDirection.Descending));
            invoiceView.Filter = x => DoFilterInvoice(x as InvoiceViewModel);
            invoiceView.CollectionChanged += InvoiceView_CollectionChanged;
            invoiceDataGrid.ItemsSource = invoiceView;

            filtersInvoice = new FilterGroup();
            filtersInvoice.PropertyChanged += FiltersInvoice_PropertyChanged;
            filtersInvoiceGroup.DataContext = filtersInvoice;

            totalsInvoice = new TotalGroup();
            totalsInvoiceGroup.DataContext = totalsInvoice;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            LoadData();
            CalculateInvoiceTotals();
        }

        private void LoadData()
        {
            List<Task> dataTasks = new List<Task>();
            dataTasks.Add(LoadDataAsync(invoiceList, CollectionScheduleHelper.GetInvoiceListAsync(), invoiceView));
            Task.WaitAll(dataTasks.ToArray());
        }

        private async Task LoadDataAsync<T>(List<T> list, Task<IEnumerable<T>> task, ICollectionView view)
        {
            list.Clear();
            var result = await task;
            list.AddRange(result);
            Dispatcher.Invoke(new Action(() => view.Refresh()), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void CalculateInvoiceTotals()
        {
            if (invoiceView == null || totalsInvoice == null || filtersInvoice == null) return;
            var filteredList = invoiceView.OfType<InvoiceViewModel>();
            totalsInvoice.TotalInvoiceCompanyCount = filteredList.GroupBy(i => i.Company).Count();
            totalsInvoice.TotalInvoiceCount = filteredList.Count();
            totalsInvoice.TotalInvoiceAmount = filteredList.Sum(i => i.Amount ?? 0);
            totalsInvoice.InvoiceMonthOf = filtersInvoice.FilterInvoiceMonth;
        }

        private void InvoiceView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CalculateInvoiceTotals();
        }

        private void FiltersInvoice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            invoiceView.Refresh();
            FilterChanged?.Invoke();
        }

        private bool DoFilterInvoice(InvoiceViewModel i)
        {
            bool flag = true;
            //invoice month
            if (filtersInvoice.FilterInvoiceMonth != 0)
                flag &= i.InvoiceMonth == filtersInvoice.FilterInvoiceMonth;
            //invoice year
            flag &= i.InvoiceYear == filtersInvoice.FilterInvoiceYear;
            return flag;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            string file = FileExtension.GetFullFileName(sender as FrameworkElement);
            if (System.IO.File.Exists(file))
                FileHelper.Open(file);
            else
                System.Windows.MessageBox.Show("ERROR: File not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ExportInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = this.FilteredInvoiceList.Where(i => i.ComparisonMark);
            if (list.Count() > 0)
                ExcelHelper.ExportMissingInvoiceList(list);
        }
    }
}
