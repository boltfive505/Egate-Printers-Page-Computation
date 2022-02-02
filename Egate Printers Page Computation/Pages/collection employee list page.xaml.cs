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
using bolt5.ModalWpf;
using bolt5.CloneCopy;

namespace Egate_Printers_Page_Computation.Pages
{
    /// <summary>
    /// Interaction logic for collection_employee_list_page.xaml
    /// </summary>
    public partial class collection_employee_list_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterKeyword { get; set; }
            public bool ShowInActive { get; set; }
        }

        private ICollectionView employeesView;
        private List<CollectionEmployeeViewModel> employeeList = new List<CollectionEmployeeViewModel>();
        private FilterGroup filters;

        public collection_employee_list_page()
        {
            InitializeComponent();
            employeesView = new CollectionViewSource() { Source = employeeList }.View;
            employeesView.SortDescriptions.Add(new SortDescription("IsActive", ListSortDirection.Descending));
            employeesView.SortDescriptions.Add(new SortDescription("EmployeeName", ListSortDirection.Ascending));
            employeesView.Filter = x => DoFilterEmployee(x as CollectionEmployeeViewModel);
            employeesDataGrid.ItemsSource = employeesView;

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
            employeeList.Clear();
            employeeList.AddRange(CollectionScheduleHelper.GetEmployeeListAsync().GetResult());
            employeesView.Refresh();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            employeesView.Refresh();
        }

        private bool DoFilterEmployee(CollectionEmployeeViewModel i)
        {
            bool flag = true;
            //keyword
            if (!string.IsNullOrWhiteSpace(filters.FilterKeyword))
            {
                string keyword = filters.FilterKeyword.Trim();
                flag &= i.EmployeeName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            //show inactive
            if (!filters.ShowInActive)
                flag &= i.IsActive;
            return flag;
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Employee";
            var employee = (sender as FrameworkElement).DataContext as CollectionEmployeeViewModel;
            if (employee == null)
            {
                isEdit = false;
                title = "Add Employee";
                employee = new CollectionEmployeeViewModel();
            }
            var modal = new employee_add_modal();
            var clone = employee.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(employee);
                _ = CollectionScheduleHelper.AddEmployeeAsync(employee);
                if (!isEdit)
                    employeeList.Add(employee);
                employeesView.Refresh();
            }
        }
    }
}
