using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Egate_Printers_Page_Computation.Objects;
using Egate_Printers_Page_Computation.Helpers;

namespace Egate_Printers_Page_Computation.SubModals
{
    /// <summary>
    /// Interaction logic for contract_add_modal.xaml
    /// </summary>
    public partial class contract_add_modal : UserControl
    {
        public contract_add_modal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<CompanyViewModel> companyList = new ObservableCollection<CompanyViewModel>(PrintersPageComputationHelper.GetCompanyListAsync().GetResult());
            CompanyValue.ItemsSource = companyList;
        }
    }
}
