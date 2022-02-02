using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Printers_Page_Computation.Helpers;
using Egate_Printers_Page_Computation.Objects.CollectionSchedule;
using Egate_Printers_Page_Computation.SubModals.CollectionSchedule;
using CustomControls.Modal;

namespace Egate_Printers_Page_Computation.Pages
{
    /// <summary>
    /// Interaction logic for collection_employee_list_page.xaml
    /// </summary>
    public partial class collection_company_location_list_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterKeyword { get; set; }
        }

        private ICollectionView locationView;
        private List<CollectionLocationViewModel> locationList = new List<CollectionLocationViewModel>();
        private FilterGroup filters;

        public collection_company_location_list_page()
        {
            InitializeComponent();
            locationView = new CollectionViewSource() { Source = locationList }.View;
            locationView.SortDescriptions.Add(new SortDescription("LocationName", ListSortDirection.Ascending));
            locationView.Filter = x => DoFilterEmployee(x as CollectionLocationViewModel);
            employeesDataGrid.ItemsSource = locationView;

            filters = new FilterGroup();
            filters.PropertyChanged += Filters_PropertyChanged;
            filtersGroup.DataContext = filters;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            locationList.Clear();
            locationList.AddRange(CollectionScheduleHelper.GetLocationListAsync().GetResult());
            locationView.Refresh();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            locationView.Refresh();
        }

        private bool DoFilterEmployee(CollectionLocationViewModel i)
        {
            bool flag = true;
            //keyword
            if (!string.IsNullOrWhiteSpace(filters.FilterKeyword))
            {
                string keyword = filters.FilterKeyword.Trim();
                flag &= i.LocationName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            return flag;
        }

        private void AddLocation_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Location";
            var location = (sender as FrameworkElement).DataContext as CollectionLocationViewModel;
            if (location == null)
            {
                isEdit = false;
                title = "Add Location";
                location = new CollectionLocationViewModel();
            }
            var modal = new location_add_modal();
            var clone = location.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(location);
                _ = CollectionScheduleHelper.AddLocationAsync(location);
                if (!isEdit)
                    locationList.Add(location);
                locationView.Refresh();
            }
        }
    }
}
