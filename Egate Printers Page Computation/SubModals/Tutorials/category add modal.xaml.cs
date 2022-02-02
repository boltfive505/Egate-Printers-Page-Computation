using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Egate_Printers_Page_Computation.Objects.Tutorials;
using Egate_Printers_Page_Computation.Helpers;
using System.ComponentModel;
using System.Windows.Data;

namespace Egate_Printers_Page_Computation.SubModals.Tutorials
{
    /// <summary>
    /// Interaction logic for category_add_modal.xaml
    /// </summary>
    public partial class category_add_modal : UserControl
    {
        public category_add_modal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            List<TutorialCategoryViewModel> categoryList = TutorialsHelper.GetCategoryListAsync().GetResult().ToList();
            TutorialsHelper.SetCategoryHierarchy(categoryList);
            categoryCbox.ItemsSource = new ObservableCollection<TutorialCategoryViewModel>(categoryList.OrderBy(i => i.PathName));
            //ICollectionView categoryView = new CollectionViewSource() { Source = categoryList }.View;
            //categoryView.SortDescriptions.Add(new SortDescription("PathName", ListSortDirection.Ascending));
            //categoryCbox.ItemsSource = categoryView;
            //categoryView.Refresh();
        }
    }
}
