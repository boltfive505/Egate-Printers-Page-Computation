using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using bolt5.CustomControls;
using bolt5.ModalWpf;

namespace Egate_Printers_Page_Computation.SubModals.CollectionSchedule
{
    /// <summary>
    /// Interaction logic for invoice_add_modal.xaml
    /// </summary>
    public partial class invoice_add_modal : UserControl, IModalClosing
    {
        public string SelfInvoiceNumber { get; set; }

        public invoice_add_modal()
        {
            InitializeComponent();
        }

        public void ModalClosing(ModalClosingArgs e)
        {
            if (e.Result == ModalResult.Save)
            {
                string errorMsg = null;
                //check for invoice number errors
                string invoiceNumber = InvoiceNumberValue.Text;
                if (string.IsNullOrWhiteSpace(invoiceNumber))
                {
                    errorMsg = "Please input Invoice Number";
                }
                else if (invoiceNumber == this.SelfInvoiceNumber)
                {
                    return; //do nothing
                }
                else
                {
                    //check for duplicate invoice number
                    if (Helpers.CollectionScheduleHelper.CheckDuplicateInvoiceNumber(invoiceNumber))
                        errorMsg = string.Format("Invoice Number '{0}' already exist", invoiceNumber);
                }
                if (errorMsg != null)
                {
                    e.Cancel = true;
                    System.Windows.MessageBox.Show(errorMsg, "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
