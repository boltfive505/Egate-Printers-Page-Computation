using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Printers_Page_Computation.Objects;
using Egate_Printers_Page_Computation.Helpers;
using CustomControls;
using CustomControls.Modal;

namespace Egate_Printers_Page_Computation.SubModals
{
    /// <summary>
    /// Interaction logic for counter_add_modal.xaml
    /// </summary>
    public partial class counter_add_modal : UserControl, IModalClosing
    {
        public static readonly DependencyProperty NewUnitCounterProperty = DependencyProperty.Register(nameof(NewUnitCounter), typeof(PageCounterViewModel), typeof(counter_add_modal));
        public PageCounterViewModel NewUnitCounter
        { 
            get { return (PageCounterViewModel)GetValue(NewUnitCounterProperty); }
            set { SetValue(NewUnitCounterProperty, value); }
        }

        public static readonly DependencyProperty LastPeriodCounterProperty = DependencyProperty.Register(nameof(LastPeriodCounter), typeof(PageCounterViewModel), typeof(counter_add_modal), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLastPeriodCounterPropertyChanged)));
        public PageCounterViewModel LastPeriodCounter
        {
            get { return (PageCounterViewModel)GetValue(LastPeriodCounterProperty); }
            set { SetValue(LastPeriodCounterProperty, value); }
        }

        public counter_add_modal()
        {
            InitializeComponent();
            NewUnitCounter = new PageCounterViewModel();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GetLastCounter();
        }

        private void GetLastCounter()
        {
            if (!(this.DataContext as PageCounterViewModel).IsNew) return; //if editing mode, return
            DateTime? date = CounterDateValue.Value;
            if (date == null)
            {
                LastPeriodCounter = null;
                return;
            }
            int contractId = (DataContext as PageCounterViewModel).Contract.Id;
            //get previous period counter
            Task.Run(async () =>
            {
                var lastCounter = await PrintersPageComputationHelper.GetPreviousPeriodCounterAsync(contractId, date.Value);
                await Dispatcher.BeginInvoke(new Action(() => LastPeriodCounter = lastCounter), System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private void CounterDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GetLastCounter();
        }

        private void UpdateLastPeriodValues()
        {
            if (!LastPeriodCounter.IsReplaced)
            {
                OldUnitNameValue.SetValue(TextBox.TextProperty, LastPeriodCounter?.UnitName);
                OldSerialNumberValue.SetValue(TextBox.TextProperty, LastPeriodCounter?.UnitSerialNumber);
            }
        }

        private static void OnLastPeriodCounterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is counter_add_modal)
            {
                (sender as counter_add_modal).UpdateLastPeriodValues();
            }
        }

        public void ModalClosing(ModalClosingArgs e)
        {
            if (e.Result == ModalResult.Save)
            {
                if (LastPeriodCounter != null && !LastPeriodCounter.IsReplaced)
                {
                    PageCounterViewModel currentCounter = DataContext as PageCounterViewModel;
                    //check if last page is more that current count, in each instance
                    if (!IsPageCountValid(LastPeriodCounter?.PageCount_Black_A4, currentCounter?.PageCount_Black_A4))
                    {
                        MessageBox.Show("Invalid Page Count for Black A4", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                    }
                    else if (!IsPageCountValid(LastPeriodCounter?.PageCount_Black_A3, currentCounter?.PageCount_Black_A3))
                    {
                        MessageBox.Show("Invalid Page Count for Black A3", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                    }
                    else if (!IsPageCountValid(LastPeriodCounter?.PageCount_Colored_A4, currentCounter?.PageCount_Colored_A4))
                    {
                        MessageBox.Show("Invalid Page Count for Colored A4", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                    }
                    else if (!IsPageCountValid(LastPeriodCounter?.PageCount_Colored_A3, currentCounter?.PageCount_Colored_A3))
                    {
                        MessageBox.Show("Invalid Page Count for Colored A3", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                    }
                }
            }
            else if (e.Result == ModalResult.Delete)
            {
                e.Cancel = (MessageBox.Show("Do you want to DELETE this page count?", "Page Counter", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes);
            }
        }

        private bool IsPageCountValid(int? lastCount, int? currentCount)
        {
            if (lastCount != null && currentCount != null)
                return (currentCount ?? 0) >= (lastCount ?? 0);
            return true;
        }

        private void OpenCamera_Click(object sender, RoutedEventArgs e)
        {
            FileAttachment fileAttachment = (sender as FrameworkElement).Tag as FileAttachment;
            var modal = new Templates.open_camera_modal();
            if (ModalForm.ShowModal(modal, "Capture Image", ModalButtons.SaveCancel) ==  ModalResult.Save)
            {
                fileAttachment.SetValue(FileAttachment.FileNameProperty, modal.GetCapturedImageFile());
            }
        }
    }
}
