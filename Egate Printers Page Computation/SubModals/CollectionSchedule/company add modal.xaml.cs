using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Egate_Printers_Page_Computation.Objects.CollectionSchedule;
using Egate_Printers_Page_Computation.Helpers;

namespace Egate_Printers_Page_Computation.SubModals.CollectionSchedule
{
    /// <summary>
    /// Interaction logic for company_add_modal.xaml
    /// </summary>
    public partial class company_add_modal : UserControl
    {
        public company_add_modal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LocationValue.ItemsSource = new ObservableCollection<CollectionLocationViewModel>(CollectionScheduleHelper.GetLocationListAsync().GetResult());
        }
    }
}
