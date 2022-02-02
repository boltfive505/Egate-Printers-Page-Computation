using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Egate_Printers_Page_Computation.Helpers;
using Egate_Printers_Page_Computation.Objects.Tutorials;

namespace Egate_Printers_Page_Computation.SubModals.Tutorials
{
    /// <summary>
    /// Interaction logic for video_add_modal.xaml
    /// </summary>
    public partial class video_add_modal : UserControl
    {
        public video_add_modal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //category
            List<TutorialCategoryViewModel> categoryList = TutorialsHelper.GetCategoryListAsync().GetResult().ToList();
            TutorialsHelper.SetCategoryHierarchy(categoryList);
            categoryCbox.ItemsSource = new ObservableCollection<TutorialCategoryViewModel>(categoryList.OrderBy(i => i.PathName));

            //employee
            List<TutorialEmployeeViewModel> employeeList = TutorialsHelper.GetEmployeeListAsync().GetResult().ToList();
            employeeList.Add(new TutorialEmployeeViewModel());
            AssignedToValue.ItemsSource = new ObservableCollection<TutorialEmployeeViewModel>(employeeList.OrderBy(i => i.EmployeeName));
        }
    }
}
