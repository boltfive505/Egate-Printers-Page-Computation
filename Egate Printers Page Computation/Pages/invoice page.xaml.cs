using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CustomControls.Modal;
using Egate_Printers_Page_Computation.SubModals.CollectionSchedule;
using Egate_Printers_Page_Computation.Objects.CollectionSchedule;
using Egate_Printers_Page_Computation.Helpers;
using System.Collections.ObjectModel;

namespace Egate_Printers_Page_Computation.Pages
{
    /// <summary>
    /// Interaction logic for collection_page.xaml
    /// </summary>
    public partial class invoice_page : Page
    {
        public class SelectionGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int? SelectedCompany { get; set; }
        }

        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            //for company
            public string FilterCompanyName { get; set; }
            public bool ShowInactiveCompany { get; set; }
            //for invoice
            public string FilterInvoiceNumber { get; set; }
            public int FilterLocation { get; set; } = -1;
            public int FilterInvoiceYear { get; set; }
            public int FilterInvoiceMonth { get; set; }
            public CompanyClientType FilterClientType { get; set; } = CompanyClientType.Rental;
            public bool ShowForSubmit { get; set; }

            public ObservableCollection<CollectionLocationViewModel> LocationList { get; set; } = new ObservableCollection<CollectionLocationViewModel>();

            public FilterGroup()
            {
                DateTime now = DateTime.Now;
                FilterInvoiceYear = now.Year;
            }

            public void Reset()
            {
                //for company
                FilterCompanyName = string.Empty;
                ShowInactiveCompany = false;
                //for invoice
                FilterInvoiceNumber = string.Empty;
                FilterLocation = -1;
                FilterInvoiceYear = DateTime.Now.Year;
                FilterInvoiceMonth = 0;
                FilterClientType = CompanyClientType.None;
                ShowForSubmit = false;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            //for company
            public int TotalActiveCompany { get; set; }
            public int TotalMissingCompany { get; set; }
            //for invoice
            public int TotalInvoiceCompanyCount { get; set; }
            public int TotalInvoiceCount { get; set; }
            public decimal TotalInvoiceAmount { get; set; }
            public int InvoiceMonthOf { get; set; }
        }

        public static readonly DependencyProperty SelectionsProperty = DependencyProperty.Register(nameof(Selections), typeof(SelectionGroup), typeof(invoice_page));
        public SelectionGroup Selections
        {
            get { return (SelectionGroup)GetValue(SelectionsProperty); }
            set { SetValue(SelectionsProperty, value); }
        }

        private ICollectionView companyView;
        private ICollectionView invoiceView;
        private List<CollectionCompanyViewModel> companyList = new List<CollectionCompanyViewModel>();
        private List<InvoiceViewModel> invoiceList = new List<InvoiceViewModel>();
        private FilterGroup filters, filtersInvoice;
        private TotalGroup totals, totalsInvoice;

        public invoice_page()
        {
            InitializeComponent();
            companyView = new CollectionViewSource() { Source = companyList }.View;
            companyView.SortDescriptions.Add(new SortDescription("CompanyName", ListSortDirection.Ascending));
            companyView.Filter = x => DoFilterCompany(x as CollectionCompanyViewModel);
            companyView.CollectionChanged += CompanyView_CollectionChanged;
            companyDataGrid.ItemsSource = companyView;

            invoiceView = new CollectionViewSource() { Source = invoiceList }.View;
            invoiceView.SortDescriptions.Add(new SortDescription("Company.CompanyName", ListSortDirection.Ascending));
            invoiceView.SortDescriptions.Add(new SortDescription("InvoiceDate", ListSortDirection.Descending));
            invoiceView.Filter = x => DoFilterInvoice(x as InvoiceViewModel);
            invoiceView.CollectionChanged += InvoiceView_CollectionChanged;
            invoiceDataGrid.ItemsSource = invoiceView;

            Selections = new SelectionGroup();
            Selections.PropertyChanged += Selections_PropertyChanged;

            filters = new FilterGroup();
            filters.PropertyChanged += Filters_PropertyChanged;
            filtersGroup.DataContext = filters;

            filtersInvoice = new FilterGroup();
            filtersInvoice.PropertyChanged += FiltersInvoice_PropertyChanged;
            filtersInvoiceGroup.DataContext = filtersInvoice;

            totals = new TotalGroup();
            totalsGroup.DataContext = totals;

            totalsInvoice = new TotalGroup();
            totalsInvoiceGroup.DataContext = totalsInvoice;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            CalculateTotals();
            CalculateInvoiceTotals();
            GetFilterList();
            CheckForMissingCompany();
            GetInvoiceIsPaid();
        }

        private void LoadData()
        {
            List<Task> dataTasks = new List<Task>();
            dataTasks.Add(LoadDataAsync(companyList, CollectionScheduleHelper.GetCompanyListAsync(), companyView));
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

        private void CalculateTotals()
        {
            var filteredList = companyView.OfType<CollectionCompanyViewModel>();
            totals.TotalActiveCompany = filteredList.Count();
            totals.TotalMissingCompany = filteredList.Count(i => i.IsMissingMark);
        }

        private void CalculateInvoiceTotals()
        {
            var filteredList = invoiceView.OfType<InvoiceViewModel>();
            totalsInvoice.TotalInvoiceCompanyCount = filteredList.GroupBy(i => i.Company).Count();
            totalsInvoice.TotalInvoiceCount = filteredList.Count(i => !i.IsVoid);
            totalsInvoice.TotalInvoiceAmount = filteredList.Sum(i => i.Amount ?? 0);
            totalsInvoice.InvoiceMonthOf = filtersInvoice.FilterInvoiceMonth;
        }

        private void GetFilterList()
        {
            var locationList = CollectionScheduleHelper.GetLocationListAsync().GetResult();
            //for company
            filters.LocationList.Clear();
            filters.LocationList = new ObservableCollection<CollectionLocationViewModel>(locationList);
            filters.LocationList.Insert(0, new CollectionLocationViewModel() { Id = -1 });
            //for invoice
            filtersInvoice.LocationList.Clear();
            filtersInvoice.LocationList = new ObservableCollection<CollectionLocationViewModel>(locationList);
            filtersInvoice.LocationList.Insert(0, new CollectionLocationViewModel() { Id = -1 });
        }

        private void CheckForMissingCompany()
        {
            List<CollectionCompanyViewModel> companyCompareList = companyView.OfType<CollectionCompanyViewModel>().ToList();
            List<InvoiceViewModel> invoiceCompareList = invoiceView.OfType<InvoiceViewModel>().ToList();
            var task = Task.Run(() =>
            {
                //check if company is missing on the invoice list
                companyCompareList.ForEach(i =>
                {
                    i.IsMissingMark = !invoiceCompareList.Any(inv => inv.Company.Id == i.Id);
                });
            });
            task.ContinueWith(t =>
            {
                CalculateTotals();
            });
        }

        private void GetInvoiceIsPaid()
        {
            Task.Run(async () =>
            {
                var collectionList = await CollectionScheduleHelper.GetScheduleListAsync();
                List<string> collectedInvoiceNumbers = new List<string>();
                foreach (var collection in collectionList.Where(i => i.IsCollected))
                {
                    collectedInvoiceNumbers.AddRange(collection.InvoiceNumber.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                }
                foreach (var invoice in invoiceList)
                    invoice.IsPaid = collectedInvoiceNumbers.Contains(invoice.InvoiceNumber);
            });
        }

        private void InvoiceView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CalculateInvoiceTotals();
            CheckForMissingCompany();
        }

        private void CompanyView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CalculateTotals();
            CheckForMissingCompany();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            companyView.Refresh();
            CalculateTotals();
            CheckForMissingCompany();
        }

        private void FiltersInvoice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            invoiceView.Refresh();
            CalculateInvoiceTotals();
            CheckForMissingCompany();
        }

        private void Selections_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            invoiceView.Refresh();
            CalculateInvoiceTotals();
            CheckForMissingCompany();
        }

        private bool DoFilterCompany(CollectionCompanyViewModel i)
        {
            bool flag = true;
            //company name
            if (!string.IsNullOrWhiteSpace(filters.FilterCompanyName))
            {
                string keyword = filters.FilterCompanyName.Trim();
                flag &= i.CompanyName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            //location
            if (filters.FilterLocation != -1)
                flag &= (i.Location?.Id ?? -1) == filters.FilterLocation;
            //client type
            if (filters.FilterClientType != CompanyClientType.None)
                flag &= i.ClientType == filters.FilterClientType;
            //show inactive
            if (!filters.ShowInactiveCompany)
                flag &= i.IsActive;
            return flag;
        }

        private bool DoFilterInvoice(InvoiceViewModel i)
        {
            bool flag = true;
            //invoice #
            if (!string.IsNullOrWhiteSpace(filtersInvoice.FilterInvoiceNumber))
                flag &= i.InvoiceNumber.StartsWith(filtersInvoice.FilterInvoiceNumber.Trim(), StringComparison.InvariantCultureIgnoreCase);
            //company location
            if (filtersInvoice.FilterLocation != -1)
                flag &= (i.Company.Location?.Id ?? -1) == filtersInvoice.FilterLocation;
            //invoice month
            if (filtersInvoice.FilterInvoiceMonth != 0)
                flag &= i.InvoiceMonth == filtersInvoice.FilterInvoiceMonth;
            //invoice year
            flag &= i.InvoiceYear == filtersInvoice.FilterInvoiceYear;
            //client type
            if (filtersInvoice.FilterClientType != CompanyClientType.None)
                flag &= i.Company.ClientType == filtersInvoice.FilterClientType;
            //for submit
            if (filtersInvoice.ShowForSubmit)
                flag &= !string.IsNullOrEmpty(i.InvoiceNumber) && (i.Amount ?? 0) != 0 && string.IsNullOrEmpty(i.FileAttachment);
            //selection
            if (Selections.SelectedCompany != null)
                flag &= i.Company.Id == Selections.SelectedCompany;
            return flag;
        }

        private void ResetSelectCompany_Click(object sender, RoutedEventArgs e)
        {
            Selections.SelectedCompany = null;
            filtersInvoice.Reset();
            invoiceView.SortDescriptions.Clear();
            invoiceView.SortDescriptions.Add(new SortDescription("Company.CompanyName", ListSortDirection.Ascending));
            invoiceView.SortDescriptions.Add(new SortDescription("InvoiceDate", ListSortDirection.Descending));
        }

        private void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Company";
            var company = (sender as FrameworkElement).DataContext as CollectionCompanyViewModel;
            if (company == null)
            {
                company = new CollectionCompanyViewModel();
                isEdit = false;
                title = "Add Company";
            }
            var modal = new company_add_modal();
            var clone = company.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(company);
                _ = CollectionScheduleHelper.AddCompanyAsync(company);
                if (!isEdit)
                    companyList.Add(company);
                companyView.Refresh();
            }
        }

        private void AddInvoice_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Invoice";
            var invoice = (sender as FrameworkElement).DataContext as InvoiceViewModel;
            DateTime now = DateTime.Now;
            if (invoice == null)
            {
                isEdit = false;
                title = "Add Invoice";
                invoice = new InvoiceViewModel();
                invoice.Company = (sender as FrameworkElement).DataContext as CollectionCompanyViewModel;
                invoice.InvoiceMonth = now.Month;
                invoice.InvoiceYear = now.Year;
            }
            var modal = new invoice_add_modal();
            var clone = invoice.DeepClone();
            clone.UpdatedDate = now;
            modal.DataContext = clone;
            modal.SelfInvoiceNumber = clone.InvoiceNumber;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(invoice);
                _ = CollectionScheduleHelper.AddInvoiceAsync(invoice);
                if (!isEdit)
                    invoiceList.Add(invoice);
                invoiceView.Refresh();
                GetInvoiceIsPaid();
            }
        }

        private void CompanyDetails_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var company = (sender as FrameworkElement).DataContext as CollectionCompanyViewModel;
            companyDetailsPopup.DataContext = company;
            companyDetailsPopup.PlacementTarget = (sender as UIElement);
            companyDetailsPopup.IsOpen = true;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            string file = FileExtension.GetFullFileName(sender as FrameworkElement);
            if (System.IO.File.Exists(file))
                FileHelper.Open(file);
            else
                System.Windows.MessageBox.Show("ERROR: File not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void UploadInvoiceFile_Click(object sender, RoutedEventArgs e)
        {
            var invoice = (sender as FrameworkElement).DataContext as InvoiceViewModel;
            Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
            open.Title = "Upload Invoice File";
            open.Multiselect = false;
            if (open.ShowDialog() == true)
            {
                invoice.FileAttachment = open.FileName;
                _ = CollectionScheduleHelper.AddInvoiceAsync(invoice);
            }
        }

        private void OpenCameraInvoiceFile_Click(object sender, RoutedEventArgs e)
        {
            var invoice = (sender as FrameworkElement).DataContext as InvoiceViewModel;
            var modal = new Templates.open_camera_modal();
            if (ModalForm.ShowModal(modal, "Capture Image for Invoice File", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                invoice.FileAttachment = modal.GetCapturedImageFile();
                _ = CollectionScheduleHelper.AddInvoiceAsync(invoice);
            }
        }

        private void RemoveInvoiceFile_Click(object sender, RoutedEventArgs e)
        {
            var invoice = (sender as FrameworkElement).DataContext as InvoiceViewModel;
            invoice.FileAttachment = null;
            _ = CollectionScheduleHelper.AddInvoiceAsync(invoice);
        }

        public void SelectCompanyWithMissingInvoice(int companyId)
        {
            filters.Reset();
            filtersInvoice.Reset();
            Selections.SelectedCompany = companyId;
            filtersInvoice.ShowForSubmit = true;
        }

        private void ExportInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = invoiceView.OfType<InvoiceViewModel>();
            if (list.Count() > 0)
                ExcelHelper.ExportInvoiceList(list, filtersInvoice.FilterInvoiceYear, filtersInvoice.FilterInvoiceMonth);
        }

        private void ExportCompany_Click(object sender, RoutedEventArgs e)
        {
            var list = companyView.OfType<CollectionCompanyViewModel>().Where(i => i.IsMissingMark);
            if (list.Count() > 0)
                ExcelHelper.ExportCompanySimpleList(list, "company issue invoice");
        }
    }
}
