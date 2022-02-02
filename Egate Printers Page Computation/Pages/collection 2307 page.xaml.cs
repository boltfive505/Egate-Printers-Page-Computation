using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using bolt5.ModalWpf;
using Egate_Printers_Page_Computation.SubModals.CollectionSchedule;
using Egate_Printers_Page_Computation.Objects.CollectionSchedule;
using Egate_Printers_Page_Computation.Helpers;
using System.Collections.ObjectModel;
using bolt5.CloneCopy;

namespace Egate_Printers_Page_Computation.Pages
{
    /// <summary>
    /// Interaction logic for collection_page.xaml
    /// </summary>
    public partial class collection_2307_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterCompanyName { get; set; }
            public int FilterLocation { get; set; } = -1;
            public DateTime? FilterDateFrom { get; set; }
            public DateTime? FilterDateTo { get; set; }
            public bool ShowIncomplete2307 { get; set; }

            public ObservableCollection<CollectionLocationViewModel> LocationList { get; set; } = new ObservableCollection<CollectionLocationViewModel>();

            public bool CanRefresh { get; set; } = true;

            public void Reset()
            {
                CanRefresh = false;
                FilterCompanyName = string.Empty;
                FilterLocation = -1;
                FilterDateFrom = null;
                FilterDateTo = null;
                ShowIncomplete2307 = false;
                CanRefresh = true;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public decimal TotalCashAmount { get; set; }
            public decimal TotalCheckAmount { get; set; }
            public decimal TotalWht { get; set; }
            public decimal TotalNetWht { get; set; }

            public void Reset()
            {
                TotalCashAmount = 0;
                TotalCheckAmount = 0;
                TotalWht = 0;
                TotalNetWht = 0;
            }
        }

        private ICollectionView collectionView;
        private List<CollectionScheduleViewModel> scheduleList = new List<CollectionScheduleViewModel>();
        private FilterGroup filters;
        private TotalGroup totals;

        public collection_2307_page()
        {
            InitializeComponent();

            collectionView = new CollectionViewSource() { Source = scheduleList }.View;
            collectionView.SortDescriptions.Add(new SortDescription("Company.CompanyName", ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new SortDescription("CollectedDate", ListSortDirection.Descending));
            collectionView.Filter = x => DoFilterCollection(x as CollectionScheduleViewModel);
            collectionDataGrid.ItemsSource = collectionView;

            filters = new FilterGroup();
            filters.PropertyChanged += Filters_PropertyChanged;
            filtersGroup.DataContext = filters;

            totals = new TotalGroup();
            totalsGrid.DataContext = totals;
            //set totals grid columns
            for (int c = 0; c < collectionDataGrid.Columns.Count; c++)
            {
                Binding b = new Binding("Columns[" + c + "].ActualWidth");
                b.ElementName = nameof(collectionDataGrid);
                ColumnDefinition colDef = new ColumnDefinition();
                colDef.SetBinding(ColumnDefinition.WidthProperty, b);
                totalsGrid.ColumnDefinitions.Add(colDef);
            }

            collectionView.CollectionChanged += CollectionView_CollectionChanged;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            CalculateTotals();
            GetFilterList();
        }

        private void LoadData()
        {
            List<Task> dataTasks = new List<Task>();
            dataTasks.Add(LoadDataAsync(scheduleList, CollectionScheduleHelper.GetScheduleListAsync(), collectionView));
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
            totals.Reset();
            var filteredList = collectionView.OfType<CollectionScheduleViewModel>();
            if (filteredList == null) return;
            totals.TotalCashAmount = filteredList.Sum(i => i.CashAmount ?? 0);
            totals.TotalCheckAmount = filteredList.Sum(i => i.CheckAmount ?? 0);
            totals.TotalWht = filteredList.Sum(i => i.WhtAmount ?? 0);
            totals.TotalNetWht = filteredList.Sum(i => i.NetWhtAmount ?? 0);
        }

        private void GetFilterList()
        {
            filters.LocationList.Clear();
            filters.LocationList = new ObservableCollection<CollectionLocationViewModel>(CollectionScheduleHelper.GetLocationListAsync().GetResult());
            filters.LocationList.Insert(0, new CollectionLocationViewModel() { Id = -1 });
            collectionView.Refresh();
        }

        private void CollectionView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CalculateTotals();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            collectionView.Refresh();
            CalculateTotals();
        }

        private bool DoFilterCollection(CollectionScheduleViewModel i)
        {
            bool flag = true;
            //company name
            if (!string.IsNullOrWhiteSpace(filters.FilterCompanyName))
            {
                string keyword = filters.FilterCompanyName.Trim();
                flag &= i.Company.CompanyName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            //location
            if (filters.FilterLocation != -1)
            {
                flag &= (i.Company.Location?.Id ?? -1) == filters.FilterLocation;
            }
            //collected date
            if (i.CollectedDate != null && filters.FilterDateFrom != null && filters.FilterDateTo != null)
                flag &= i.CollectedDate.Value.Date >= filters.FilterDateFrom.Value.Date && i.CollectedDate.Value.Date <= filters.FilterDateTo.Value.Date;
            //incomplete 2307 info -> no file or wht amount
            if (filters.ShowIncomplete2307)
                flag &= string.IsNullOrEmpty(i.File2307Attachment) || (i.WhtAmount ?? 0) == 0;

            flag &= i.Company.IsActive; //is active
            flag &= i.CollectedDate != null; //collected
            flag &= i.Company.Is2307; //company is 2307
            return flag;
        }

        private void EditCollect_Click(object sender, RoutedEventArgs e)
        {
            var schedule = (sender as FrameworkElement).DataContext as CollectionScheduleViewModel;
            var modal = new collection_add_modal();
            modal.For2307Details = true;
            modal.IsEditMode = true;
            var clone = schedule.DeepClone();
            modal.DataContext = clone;
            var result = ModalForm.ShowModal(modal, "Edit Collection", ModalButtons.SaveCancel);
            if (result == ModalResult.Save)
            {
                clone.DeepCopyTo(schedule);
                _ = CollectionScheduleHelper.AddScheduleAsync(schedule);
                collectionView.Refresh();
            }
            else if (result == ModalResult.Delete)
            {
                _ = CollectionScheduleHelper.DeleteScheduleAsync(schedule);
                scheduleList.Remove(schedule);
                collectionView.Refresh();
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            string file = FileExtension.GetFullFileName(sender as FrameworkElement);
            if (System.IO.File.Exists(file))
                FileHelper.Open(file);
            else
                System.Windows.MessageBox.Show("ERROR: File not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ApplyDateRange_btn_Click(object sender, RoutedEventArgs e)
        {
            filters.CanRefresh = false;
            DateRangeType? dateRange = (sender as FrameworkElement).Tag as DateRangeType?;
            if (dateRange != null)
            {
                DateTime start, end;
                DateRangeHelper.GetDateRange(dateRange.Value, out start, out end);
                filters.FilterDateFrom = start;
                filters.FilterDateTo = end;
            }
            filters.CanRefresh = true;
            Filters_PropertyChanged(null, null);
        }

        private void ResetFilters_btn_Click(object sender, RoutedEventArgs e)
        {
            filters.Reset();
            Filters_PropertyChanged(null, null);
            collectionView.SortDescriptions.Clear();
            collectionView.SortDescriptions.Add(new SortDescription("Company.CompanyName", ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new SortDescription("CollectedDate", ListSortDirection.Descending));
        }

        private void ExportExcel_btn_Click(object sender, RoutedEventArgs e)
        {
            var list = collectionView.OfType<CollectionScheduleViewModel>();
            ExcelHelper.ExportCollectionList(list);
        }
    }
}
