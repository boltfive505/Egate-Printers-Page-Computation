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
using CustomControls.Modal;
using CustomControls;

namespace Egate_Printers_Page_Computation.SubModals.CollectionSchedule
{
    /// <summary>
    /// Interaction logic for collection_add_modal.xaml
    /// </summary>
    public partial class collection_add_modal : UserControl, IModalClosing
    {
        public static readonly DependencyProperty For2307DetailsProperty = DependencyProperty.Register(nameof(For2307Details), typeof(bool), typeof(collection_add_modal), new PropertyMetadata(false));
        public bool For2307Details
        {
            get { return (bool)GetValue(For2307DetailsProperty); }
            set { SetValue(For2307DetailsProperty, value); }
        }

        public bool IsEditMode { get; set; }

        public collection_add_modal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ColledtedByEmployeeValue.ItemsSource = new ObservableCollection<CollectionEmployeeViewModel>(CollectionScheduleHelper.GetEmployeeListAsync(includeInactive: false).GetResult());
            BankValue.ItemsSource = new ObservableCollection<CollectionBankViewModel>(CollectionScheduleHelper.GetBankListAsync().GetResult());
        }

        public void ModalClosing(ModalClosingArgs e)
        {
            if (!IsEditMode && e.Result == ModalResult.Save)
            {
                e.Cancel = System.Windows.MessageBox.Show("Do you confirm if Collection Information is correct?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes;
            }
            else if (e.Result == ModalResult.Delete)
            {
                e.Cancel = System.Windows.MessageBox.Show("Do you want to DELETE this Collection?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes;
            }
        }

        private void OpenCamera_Click(object sender, RoutedEventArgs e)
        {
            FileAttachment fileAttachment = (sender as FrameworkElement).Tag as FileAttachment;
            var modal = new Templates.open_camera_modal();
            if (ModalForm.ShowModal(modal, "Capture Image", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                fileAttachment.SetValue(FileAttachment.FileNameProperty, modal.GetCapturedImageFile());
            }
        }
    }
}
