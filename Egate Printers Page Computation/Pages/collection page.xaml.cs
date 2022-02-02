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
    public partial class collection_page : Page
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
            public int FilterLocation { get; set; } = -1;
            public CompanyClientType FilterClientType { get; set; } = CompanyClientType.None;
            public bool ShowBankTransfer { get; set; }
            public bool ShowInactiveCompany { get; set; }
            //for schedule
            public bool FollowupList { get; set; }
            public bool ShowForFollowupSchedule { get; set; }
            public int ForFollowupScheduleDays { get; set; } = 7;
            public ScheduleNotesType FilterNotesType { get; set; } = ScheduleNotesType.None;

            public ObservableCollection<CollectionEmployeeViewModel> EmployeeList { get; set; } = new ObservableCollection<CollectionEmployeeViewModel>();
            public ObservableCollection<CollectionLocationViewModel> LocationList { get; set; } = new ObservableCollection<CollectionLocationViewModel>();

            public bool CanRefresh { get; set; } = true;

            public void ResetCompany()
            {
                CanRefresh = false;
                //for company
                FilterCompanyName = string.Empty;
                FilterLocation = -1;
                FilterClientType = CompanyClientType.None;
                ShowBankTransfer = false;
                ShowInactiveCompany = false;
                //for schedule
                FollowupList = false;
                ShowForFollowupSchedule = false;
                FilterNotesType = ScheduleNotesType.None;
                CanRefresh = true;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int TotalActiveCompany { get; set; }
            public int TotalSchedules { get; set; }
        }

        public static readonly DependencyProperty SelectionsProperty = DependencyProperty.Register(nameof(Selections), typeof(SelectionGroup), typeof(collection_page));
        public SelectionGroup Selections
        {
            get { return (SelectionGroup)GetValue(SelectionsProperty); }
            set { SetValue(SelectionsProperty, value); }
        }

        private ICollectionView companyView;
        private ICollectionView collectionView;
        private List<CollectionCompanyViewModel> companyList = new List<CollectionCompanyViewModel>();
        private List<CollectionScheduleViewModel> scheduleList = new List<CollectionScheduleViewModel>();
        private FilterGroup filters, scheduleFilters;
        private TotalGroup totals;

        public collection_page()
        {
            InitializeComponent();
            companyView = new CollectionViewSource() { Source = companyList }.View;
            companyView.SortDescriptions.Add(new SortDescription("CompanyName", ListSortDirection.Ascending));
            companyView.Filter = x => DoFilterCompany(x as CollectionCompanyViewModel);
            companyDataGrid.ItemsSource = companyView;

            collectionView = new CollectionViewSource() { Source = scheduleList }.View;
            collectionView.SortDescriptions.Add(new SortDescription("CollectedDate", ListSortDirection.Descending));
            collectionView.Filter = x => DoFilterCollection(x as CollectionScheduleViewModel);
            collectionDataGrid.ItemsSource = collectionView;

            Selections = new SelectionGroup();
            Selections.PropertyChanged += Selections_PropertyChanged;

            filters = new FilterGroup();
            filters.PropertyChanged += Filters_PropertyChanged;
            filtersGroup.DataContext = filters;
            //defaultEmployeeGroup.DataContext = filters;

            scheduleFilters = new FilterGroup();
            scheduleFilters.PropertyChanged += ScheduleFilters_PropertyChanged;
            scheduleFiltersGroup.DataContext = scheduleFilters;

            totals = new TotalGroup();
            totalsGroup.DataContext = totals;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            CalculateTotals();
            GetFilterList();
            _ = CollectionScheduleHelper.GetScheduledCountAsync(companyList, scheduleList);
            _ = CollectionScheduleHelper.GetCompanyFollowUpAsync(companyList, scheduleList);
            _ = CollectionScheduleHelper.GetCompanyLatestCollectionAsync(companyList, scheduleList);

            Task currentSchedTask = CollectionScheduleHelper.GetCompanyCurrentScheduleAsync(companyList, scheduleList);
            currentSchedTask.ContinueWith(t => CalculateTotals());
        }

        private void LoadData()
        {
            List<Task> dataTasks = new List<Task>();
            dataTasks.Add(LoadDataAsync(companyList, CollectionScheduleHelper.GetCompanyListAsync(), companyView));
            dataTasks.Add(LoadDataAsync(scheduleList, CollectionScheduleHelper.GetScheduleListAsync(), collectionView));
            Task.WaitAll(dataTasks.ToArray());
            //collectionView.Refresh();
        }

        private async Task LoadDataAsync<T>(List<T> list, Task<IEnumerable<T>> task, ICollectionView view)
        {
            list.Clear();
            var result = await task;
            list.AddRange(result);
            _ = Dispatcher.BeginInvoke(new Action(() => view.Refresh()), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void CalculateTotals()
        {
            totals.TotalActiveCompany = companyView.OfType<CollectionCompanyViewModel>().Count();
            totals.TotalSchedules = companyView.OfType<CollectionCompanyViewModel>().Count(i => i.CurrentScheduledCollection != null);
        }

        private void GetFilterList()
        {
            filters.LocationList.Clear();
            filters.EmployeeList.Clear();
            filters.LocationList = new ObservableCollection<CollectionLocationViewModel>(CollectionScheduleHelper.GetLocationListAsync().GetResult());
            filters.LocationList.Insert(0, new CollectionLocationViewModel() { Id = -1 });
            filters.EmployeeList = new ObservableCollection<CollectionEmployeeViewModel>(CollectionScheduleHelper.GetEmployeeListAsync(false).GetResult());
            companyView.Refresh();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            companyView.Refresh();
            CalculateTotals();
        }

        private void Selections_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            collectionView.Refresh();
        }

        private void ScheduleFilters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(scheduleFilters.FollowupList))
            {
                companyView.SortDescriptions.Clear();
                if (scheduleFilters.FollowupList)
                {
                    companyView.SortDescriptions.Add(new SortDescription("UpdatedDate", ListSortDirection.Ascending));
                    companyView.SortDescriptions.Add(new SortDescription("LatestCollected.CollectedDate", ListSortDirection.Ascending));
                }
                else
                {
                    companyView.SortDescriptions.Add(new SortDescription("CompanyName", ListSortDirection.Ascending));
                }
            }
            companyView.Refresh();
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
            {
                flag &= (i.Location?.Id ?? -1) == filters.FilterLocation;
            }
            //show followup schedule
            if (scheduleFilters.ShowForFollowupSchedule)
            {
                if (i.CurrentScheduledCollection != null)
                {
                    DateTime forFollowupDateAsOf = DateTime.Now.Date.AddDays(-scheduleFilters.ForFollowupScheduleDays);
                    var schedule = i.CurrentScheduledCollection;
                    flag &= (schedule.ScheduleDate == null || schedule.NotScheduled) && (schedule.UpdatedDate != null && schedule.UpdatedDate.Value.Date <= forFollowupDateAsOf);
                }
                else
                {
                    flag &= false;
                }
            }
            //notes
            if (scheduleFilters.FilterNotesType != ScheduleNotesType.None)
            {
                if (i.CurrentScheduledCollection != null)
                {
                    if (scheduleFilters.FilterNotesType == ScheduleNotesType.Others)
                        flag &= !string.IsNullOrEmpty(i.CurrentScheduledCollection.ScheduleNotes);
                    else
                        flag &= i.CurrentScheduledCollection.ScheduleNotesType == scheduleFilters.FilterNotesType;
                }
                else
                    flag &= false;
            }
            //client type
            if (filters.FilterClientType != CompanyClientType.None)
                flag &= i.ClientType == filters.FilterClientType;
            //bank transfer
            if (filters.ShowBankTransfer)
                flag &= i.IsBankTransfer;
            //show inactive
            if (!filters.ShowInactiveCompany)
                flag &= i.IsActive;
            return flag;
        }

        private bool DoFilterCollection(CollectionScheduleViewModel i)
        {
            bool flag = true;
            //selection
            if (Selections.SelectedCompany != null)
                flag &= i.Company.Id == Selections.SelectedCompany;
            //collected
            flag &= i.CollectedDate != null;
            return flag;
        }

        private void ResetSelectCompany_Click(object sender, RoutedEventArgs e)
        {
            Selections.SelectedCompany = null;
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
                CalculateTotals();
            }
        }

        private void AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Schedule";
            var buttons = ModalButtons.SaveCancelDelete;
            var company = (sender as FrameworkElement).DataContext as CollectionCompanyViewModel;
            var schedule = company.CurrentScheduledCollection;
            if (schedule == null)
            {
                isEdit = false; //set for adding mode
                title = "Add Schedule";
                buttons = ModalButtons.SaveCancel;
                schedule = new CollectionScheduleViewModel();
                schedule.Company = company;
                schedule.ScheduleDate = DateTime.Now;
            }
            var modal = new schedule_add_modal();
            var clone = schedule.DeepClone();
            clone.UpdatedDate = DateTime.Now;
            modal.DataContext = clone;
            var result = ModalForm.ShowModal(modal, title, buttons);
            if (result == ModalResult.Save)
            {
                clone.DeepCopyTo(schedule);
                if (schedule.NotScheduled)
                    schedule.ScheduleDate = null;
                if (schedule.ScheduleNotesType != ScheduleNotesType.Others)
                    schedule.ScheduleNotes = null;
                _ = CollectionScheduleHelper.AddScheduleAsync(schedule);
                if (!isEdit)
                    scheduleList.Add(schedule);
                company.CurrentScheduledCollection = schedule;
                CalculateTotals();
                companyView.Refresh();
            }
            else if (result == ModalResult.Delete)
            {
                _ = CollectionScheduleHelper.DeleteScheduleAsync(schedule);
                scheduleList.Remove(schedule);
                company.CurrentScheduledCollection = null;
                CalculateTotals();
                companyView.Refresh();
            }
            
        }

        private void ClearSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Do you want to CLEAR ALL SCHEDULES?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _ = CollectionScheduleHelper.DeleteScheduleMultipleAsync(companyList.Where(i => i.CurrentScheduledCollection != null).Select(i => i.CurrentScheduledCollection)); //delete from database
                foreach (var company in companyList)
                {
                    //delete schedule
                    scheduleList.Remove(company.CurrentScheduledCollection); //remove from list
                    company.CurrentScheduledCollection = null; //set to null
                }
                CalculateTotals();
            }
        }

        private void Collect_Click(object sender, RoutedEventArgs e)
        {
            var company = (sender as FrameworkElement).DataContext as CollectionCompanyViewModel;
            var schedule = company.CurrentScheduledCollection;
            if (schedule == null)
            {
                schedule = new CollectionScheduleViewModel();
                schedule.Company = company;
            }
            if (schedule.IsCollected) return;
            var modal = new collection_add_modal();
            var clone = schedule.DeepClone();
            clone.CollectedDate = DateTime.Now;
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, "Collect Amount", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(schedule);
                _ = CollectionScheduleHelper.AddScheduleAsync(schedule);
                company.CurrentScheduledCollection = null;
                _ = CollectionScheduleHelper.GetCompanyLatestCollectionAsync(companyList, scheduleList);
                scheduleList.Add(schedule);
                CalculateTotals();
                companyView.Refresh();
                collectionView.Refresh();
            }
        }

        private void EditCollect_Click(object sender, RoutedEventArgs e)
        {
            var schedule = (sender as FrameworkElement).DataContext as CollectionScheduleViewModel;
            var modal = new collection_add_modal();
            modal.IsEditMode = true;
            var clone = schedule.DeepClone();
            modal.DataContext = clone;
            var result = ModalForm.ShowModal(modal, "Edit Collection", ModalButtons.SaveCancelDelete);
            if (result == ModalResult.Save)
            {
                clone.DeepCopyTo(schedule);
                _ = CollectionScheduleHelper.AddScheduleAsync(schedule);
                _ = CollectionScheduleHelper.GetCompanyLatestCollectionAsync(companyList, scheduleList);
                collectionView.Refresh();
            }
            else if (result == ModalResult.Delete)
            {
                _ = CollectionScheduleHelper.DeleteScheduleAsync(schedule);
                scheduleList.Remove(schedule);
                _ = CollectionScheduleHelper.GetCompanyLatestCollectionAsync(companyList, scheduleList);
                companyView.Refresh();
                collectionView.Refresh();
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

        private void ExportCompany_Click(object sender, RoutedEventArgs e)
        {
            var list = companyView.OfType<CollectionCompanyViewModel>();
            ExcelHelper.ExportCompanyList(list);
        }

        private void ResetCompanySelection_Click(object sender, RoutedEventArgs e)
        {
            Selections.SelectedCompany = null;
        }

        private void ExportSchedule_Click(object sender, RoutedEventArgs e)
        {
            var list = companyView.OfType<CollectionCompanyViewModel>()
                                .Where(i => i.CurrentScheduledCollection != null)
                                .Select(i => i.CurrentScheduledCollection);
            ExcelHelper.ExportScheduleList(list);
        }
    }

    public class CompanyScheduleTemplateSelector : DataTemplateSelector
    { 
        public DataTemplate WithScheduleTemplate { get; set; }
        public DataTemplate WithoutScheduleTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ContentPresenter presenter = container as ContentPresenter;
            DataGridCell cell = presenter.Parent as DataGridCell;
            var company = cell.DataContext as CollectionCompanyViewModel;
            if (company != null)
            {
                if (company.CurrentScheduledCollection == null) return WithoutScheduleTemplate;
                else return WithScheduleTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
