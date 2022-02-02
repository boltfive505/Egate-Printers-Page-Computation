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
using bolt5.ModalWpf;

namespace Egate_Printers_Page_Computation.SubModals.CollectionSchedule
{
    /// <summary>
    /// Interaction logic for collection_add_modal.xaml
    /// </summary>
    public partial class schedule_add_modal : UserControl, IModalClosing
    {
        private List<string> _notesPreselectionList = new List<string>();
        
        public schedule_add_modal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatedByEmployeeValue.ItemsSource = new ObservableCollection<CollectionEmployeeViewModel>(CollectionScheduleHelper.GetEmployeeListAsync(includeInactive: false).GetResult());
        }

        public void ModalClosing(ModalClosingArgs e)
        {
            if (e.Result == ModalResult.Delete)
            {
                e.Cancel = System.Windows.MessageBox.Show("Do you want to DELETE this schedule?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes;
            }
        }
    }
}
