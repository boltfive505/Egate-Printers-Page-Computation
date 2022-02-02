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
    public partial class company_list_followup_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            //for company
            public string FilterCompanyName { get; set; }
            public int FilterLocation { get; set; } = -1;
            public bool ShowInactiveCompany { get; set; }

            public ObservableCollection<CollectionLocationViewModel> LocationList { get; set; } = new ObservableCollection<CollectionLocationViewModel>();
        }

        private ICollectionView companyView;
        private List<CollectionCompanyViewModel> companyList = new List<CollectionCompanyViewModel>();
        private FilterGroup filters;

        public company_list_followup_page()
        {
            InitializeComponent();
            companyView = new CollectionViewSource() { Source = companyList }.View;
            companyView.SortDescriptions.Add(new SortDescription("CompanyName", ListSortDirection.Ascending));
            companyView.Filter = x => DoFilterCompany(x as CollectionCompanyViewModel);
            companyDataGrid.ItemsSource = companyView;

            filters = new FilterGroup();
            filters.PropertyChanged += Filters_PropertyChanged;
            filtersGroup.DataContext = filters;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            LoadFollowup();
            GetFilterList();
        }

        private void LoadData()
        {
            List<Task> dataTasks = new List<Task>();
            dataTasks.Add(LoadDataAsync(companyList, CollectionScheduleHelper.GetCompanyListAsync(), companyView));
            Task.WaitAll(dataTasks.ToArray());
        }

        private void LoadFollowup()
        {
            Task task = CollectionScheduleHelper.GetCompanyFollowupAllList(companyList);
            task.ContinueWith(t => Dispatcher.Invoke(() => companyView.Refresh(), System.Windows.Threading.DispatcherPriority.Background));
        }

        private async Task LoadDataAsync<T>(List<T> list, Task<IEnumerable<T>> task, ICollectionView view)
        {
            list.Clear();
            var result = await task;
            list.AddRange(result);
            Dispatcher.Invoke(new Action(() => view.Refresh()), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void GetFilterList()
        {
            filters.LocationList.Clear();
            filters.LocationList = new ObservableCollection<CollectionLocationViewModel>(CollectionScheduleHelper.GetLocationListAsync().GetResult());
            filters.LocationList.Insert(0, new CollectionLocationViewModel() { Id = -1 });
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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
                flag &= (i.Location?.Id ?? -1) == filters.FilterLocation;
            //show inactive
            if (!filters.ShowInactiveCompany)
                flag &= i.IsActive;
            //show has any followup
            flag &= (i.HasFollowupCollection || i.HasFollowup2307 || i.InvoiceNumberMissingList.Count > 0);
            return flag;
        }

        private void CompanyDetails_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var company = (sender as FrameworkElement).DataContext as CollectionCompanyViewModel;
            companyDetailsPopup.DataContext = company;
            companyDetailsPopup.PlacementTarget = (sender as UIElement);
            companyDetailsPopup.IsOpen = true;
        }

        private void EditToDoNotes_Click(object sender, RoutedEventArgs e)
        {
            var company = (sender as FrameworkElement).DataContext as CollectionCompanyViewModel;
            if (company == null) return;
            var modal = new to_do_notes_edit_modal();
            var clone = company.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, "Add Notes", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(company);
                _ = CollectionScheduleHelper.AddCompanyAsync(company);
                companyView.Refresh();
            }
        }

        private void SelectCompanyToInvoice_Click(object sender, RoutedEventArgs e)
        {
            var company = (sender as FrameworkElement).DataContext as CollectionCompanyViewModel;
            MainWindow.ChangeTab(sender, PageTab.Invoice, page => (page as invoice_page).SelectCompanyWithMissingInvoice(company.Id));
        }

        //private void OpenFile_Click(object sender, RoutedEventArgs e)
        //{
        //    string file = FileExtension.GetFullFileName(sender as FrameworkElement);
        //    if (System.IO.File.Exists(file))
        //        FileHelper.Open(file);
        //    else
        //        System.Windows.MessageBox.Show("ERROR: File not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
        //}
    }
}
