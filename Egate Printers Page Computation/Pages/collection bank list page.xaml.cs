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
    public partial class collection_bank_list_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterKeyword { get; set; }
        }

        private ICollectionView bankView;
        private List<CollectionBankViewModel> bankList = new List<CollectionBankViewModel>();
        private FilterGroup filters;

        public collection_bank_list_page()
        {
            InitializeComponent();
            bankView = new CollectionViewSource() { Source = bankList }.View;
            bankView.SortDescriptions.Add(new SortDescription("BankName", ListSortDirection.Ascending));
            bankView.Filter = x => DoFilterEmployee(x as CollectionBankViewModel);
            employeesDataGrid.ItemsSource = bankView;

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
            bankList.Clear();
            bankList.AddRange(CollectionScheduleHelper.GetBankListAsync().GetResult());
            bankView.Refresh();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bankView.Refresh();
        }

        private bool DoFilterEmployee(CollectionBankViewModel i)
        {
            bool flag = true;
            //keyword
            if (!string.IsNullOrWhiteSpace(filters.FilterKeyword))
            {
                string keyword = filters.FilterKeyword.Trim();
                flag &= i.BankName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            return flag;
        }

        private void AddBank_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Bank";
            var bank = (sender as FrameworkElement).DataContext as CollectionBankViewModel;
            if (bank == null)
            {
                isEdit = false;
                title = "Add Bank";
                bank = new CollectionBankViewModel();
            }
            var modal = new bank_add_modal();
            var clone = bank.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(bank);
                _ = CollectionScheduleHelper.AddBankAsync(bank);
                if (!isEdit)
                    bankList.Add(bank);
                bankView.Refresh();
            }
        }
    }
}
