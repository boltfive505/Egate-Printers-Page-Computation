using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Printers_Page_Computation.Objects;
using Egate_Printers_Page_Computation.Helpers;
using Egate_Printers_Page_Computation.SubModals;
using bolt5.ModalWpf;
using System.Collections.ObjectModel;
using bolt5.CloneCopy;

namespace Egate_Printers_Page_Computation.Pages
{
    /// <summary>
    /// Interaction logic for main_page.xaml
    /// </summary>
    public partial class page_counter_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterCompanyName { get; set; }
            public string FilterContractNumber { get; set; }
            public bool ShowInactiveCompany { get; set; }

            public bool CanSearch { get; set; } = true;

            public void Reset()
            {
                CanSearch = false;
                FilterCompanyName = string.Empty;
                FilterContractNumber = string.Empty;
                ShowInactiveCompany = false;
                CanSearch = true;
            }
        }

        public class SelectionGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int? SelectedCompany { get; set; }
            public int? SelectedContract { get; set; }
        }

        public class SelectInvoiceGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public CompanyViewModel SelectCompany { get; set; }
            public ICollectionView CompanyView { get; set; }
            public int ConsumptionMonth { get; set; }
            public int ConsumptionYear { get; set; }

            public SelectInvoiceGroup()
            {
                DateTime now = DateTime.Now;
                ConsumptionMonth = now.Month;
                ConsumptionYear = now.Year;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public decimal TotalGrandTotal { get; set; }
        }

        private ICollectionView companyView;
        private ICollectionView contractView;
        private ICollectionView pageCounterView;
        private ICollectionView computationView;
        private List<CompanyViewModel> companyList = new List<CompanyViewModel>();
        private List<ContractViewModel> contractList = new List<ContractViewModel>();
        private List<PageCounterViewModel> pageCounterList = new List<PageCounterViewModel>();
        private List<ComputationGroupViewModel> computationList = new List<ComputationGroupViewModel>();

        private FilterGroup filters;
        private SelectionGroup selections;
        private SelectInvoiceGroup selectInvoice;

        public page_counter_page()
        {
            InitializeComponent();

            companyView = new CollectionViewSource() { Source = companyList }.View;
            companyView.Filter = x => DoFilterCompany(x as CompanyViewModel);
            companyView.SortDescriptions.Add(new SortDescription("CompanyName", ListSortDirection.Ascending));
            companyView.CollectionChanged += CompanyView_CollectionChanged;
            companyDataGrid.ItemsSource = companyView;

            contractView = new CollectionViewSource() { Source = contractList }.View;
            contractView.Filter = x => DoFilterContract(x as ContractViewModel);
            contractView.SortDescriptions.Add(new SortDescription("ContractNumber", ListSortDirection.Ascending));
            contractView.CollectionChanged += ContractView_CollectionChanged;
            contractDataGrid.ItemsSource = contractView;

            pageCounterView = new CollectionViewSource() { Source = pageCounterList }.View;
            pageCounterView.Filter = x => DoFilterCounter(x as PageCounterViewModel);
            pageCounterView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            pageCounterView.CollectionChanged += PageCounterView_CollectionChanged;
            counterDataGrid.ItemsSource = pageCounterView;

            computationView = new CollectionViewSource() { Source = computationList }.View;
            computationView.Filter = x => DoFilterComputation(x as ComputationGroupViewModel);
            computationView.SortDescriptions.Add(new SortDescription("ConsumptionDate", ListSortDirection.Descending));
            savedReportsDataGrid.ItemsSource = computationView;

            filters = new FilterGroup();
            filters.PropertyChanged += FilterCompany_PropertyChanged;
            filtersGroup.DataContext = filters;
            otherFiltersGroup.DataContext = filters;

            selections = new SelectionGroup();
            selections.PropertyChanged += Selections_PropertyChanged;

            selectInvoice = new SelectInvoiceGroup();
            selectInvoice.CompanyView = new CollectionViewSource() { Source = companyList }.View;
            selectInvoiceGroup.DataContext = selectInvoice;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            SetContractCountForCompany();
            SetPageCountDifference();
        }

        private void LoadData()
        {
            List<Task> dataTasks = new List<Task>();
            dataTasks.Add(LoadDataAsync(companyList, PrintersPageComputationHelper.GetCompanyListAsync(), companyView));
            dataTasks.Add(LoadDataAsync(contractList, PrintersPageComputationHelper.GetContractListAsync(), contractView));
            dataTasks.Add(LoadDataAsync(pageCounterList, PrintersPageComputationHelper.GetPageCounterList(), pageCounterView));
            dataTasks.Add(LoadDataAsync(computationList, PrintersPageComputationHelper.GetComputationListAsync(), computationView));
            Task.WaitAll(dataTasks.ToArray());
        }

        private async Task LoadDataAsync<T>(List<T> list, Task<IEnumerable<T>> task, ICollectionView view)
        {
            list.Clear();
            var result = await task;
            list.AddRange(result);
            Dispatcher.Invoke(new Action(() => view.Refresh()), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void CompanyView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetContractCountForCompany();
            selectInvoice.CompanyView.Refresh();
        }

        private void ContractView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetContractCountForCompany();
        }

        private void PageCounterView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetPageCountDifference();
            //SetPageCountComputations();
        }

        private void SetContractCountForCompany()
        {
            Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    companyList.ForEach(i => i.ContractCount = contractList.Where(c => c.Company.Id == i.Id).Sum(c => 1));
                }), System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private void SetPageCountDifference()
        {
            var pageCountList = pageCounterView.OfType<PageCounterViewModel>().OrderByDescending(i => i.Date).ToList();
            if (pageCountList.Count > 0)
            {
                if (pageCountList.Count > 1)
                {
                    for (int i = 0; i < pageCountList.Count - 1; i++)
                    {
                        pageCountList[i].LastPageCount = pageCountList[i + 1];
                    }
                }
                pageCountList.Last().LastPageCount = null;
            }
        }

        private void FilterCompany_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (filters.CanSearch)
            {
                companyView.Refresh();
                contractView.Refresh();
                pageCounterView.Refresh();
            }
        }

        private void Selections_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (filters.CanSearch)
            {
                contractView.Refresh();
                pageCounterView.Refresh();
                computationView.Refresh();
            }
        }

        private void ResetFilter_btn_Click(object sender, RoutedEventArgs e)
        {
            filters.CanSearch = false;
            filters.Reset();
            filters.CanSearch = true;
            companyView.Refresh();
            contractView.Refresh();
            pageCounterView.Refresh();
        }

        private bool DoFilterCompany(CompanyViewModel i)
        {
            bool flag = true;
            //company name
            if (!string.IsNullOrWhiteSpace(filters.FilterCompanyName))
                flag &= i.CompanyName.IndexOf(filters.FilterCompanyName.Trim(), 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            //is active
            if (!filters.ShowInactiveCompany)
                flag &= i.IsActive;
            return flag;
        }

        private bool DoFilterContract(ContractViewModel i)
        {
            bool flag = true;
            //company selection
            if (selections.SelectedCompany != null)
                flag &= i.Company.Id == selections.SelectedCompany;
            else
                flag &= false;
            //contract number
            if (!string.IsNullOrWhiteSpace(filters.FilterContractNumber))
                flag &= i.ContractNumber.IndexOf(filters.FilterContractNumber.Trim(), 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            return flag;
        }

        private bool DoFilterCounter(PageCounterViewModel i)
        {
            bool flag = true;
            //contract selection
            if (selections.SelectedContract != null)
                flag &= i.Contract.Id == selections.SelectedContract;
            else
                flag &= false;
            return flag;
        }

        private bool DoFilterComputation(ComputationGroupViewModel i)
        {
            bool flag = true;
            //contract selection
            if (selections.SelectedContract != null)
                flag &= i.Contract.Id == selections.SelectedContract;
            else
                flag &= false;
            return flag;
        }

        private void companyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CompanyViewModel selectedCompany = e.AddedItems.OfType<CompanyViewModel>().FirstOrDefault() as CompanyViewModel;
            selections.SelectedCompany = selectedCompany?.Id;
        }

        private void contractDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ContractViewModel selectedContract = e.AddedItems.OfType<ContractViewModel>().FirstOrDefault() as ContractViewModel;
            selections.SelectedContract = selectedContract?.Id;
        }

        private void AddCompany_btn_Click(object sender, RoutedEventArgs e)
        {
            CompanyViewModel companyVm = new CompanyViewModel();
            var modal = new company_add_modal();
            modal.DataContext = companyVm;
            if (ModalForm.ShowModal(modal, "Add Company", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                _ = PrintersPageComputationHelper.AddUpdateCompany(companyVm);
                companyList.Add(companyVm);
                companyView.Refresh();
            }
        }

        private void EditCompany_btn_Click(object sender, RoutedEventArgs e)
        {
            CompanyViewModel companyVm = (sender as FrameworkElement).DataContext as CompanyViewModel;
            if (companyVm == null) return;
            var modal = new company_add_modal();
            var clone = companyVm.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, "Edit Company", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(companyVm);
                _ = PrintersPageComputationHelper.AddUpdateCompany(companyVm);
                companyView.Refresh();
            }
        }

        private void AddContract_btn_Click(object sender, RoutedEventArgs e)
        {
            CompanyViewModel selectedCompany = (sender as FrameworkElement).DataContext as CompanyViewModel;
            if (selectedCompany == null) return;
            ContractViewModel contractVm = new ContractViewModel();
            contractVm.Company = selectedCompany;
            var modal = new contract_add_modal();
            modal.DataContext = contractVm;
            if (ModalForm.ShowModal(modal, "Add Contract", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                _ = PrintersPageComputationHelper.AddUpdateContract(contractVm);
                contractList.Add(contractVm);
                contractView.Refresh();
            }
        }

        private void EditContract_btn_Click(object sender, RoutedEventArgs e)
        {
            ContractViewModel contractVm = (sender as FrameworkElement).DataContext as ContractViewModel;
            if (contractVm == null) return;
            var modal = new contract_add_modal();
            var clone = contractVm.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, "Edit Contract", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(contractVm);
                _ = PrintersPageComputationHelper.AddUpdateContract(contractVm);
                contractView.Refresh();
            }
        }

        private void AddCounter_btn_Click(object sender, RoutedEventArgs e)
        {
            ContractViewModel selectedContract = (sender as FrameworkElement).DataContext as ContractViewModel;
            if (selectedContract == null) return;
            PageCounterViewModel counterVm = new PageCounterViewModel();
            counterVm.Contract = selectedContract;
            counterVm.Date = DateTime.Now;
            var modal = new counter_add_modal();
            modal.DataContext = counterVm;
            if (ModalForm.ShowModal(modal, "Add Page Counter", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                _ = PrintersPageComputationHelper.AddUpdatePageCounter(counterVm);
                pageCounterList.Add(counterVm);
                if (counterVm.IsReplaced)
                {
                    //page counter for replacement, get new unit counter on the modal
                    modal.NewUnitCounter.Contract = counterVm.Contract;
                    modal.NewUnitCounter.Date = counterVm.Date.AddSeconds(5);
                    _ = PrintersPageComputationHelper.AddUpdatePageCounter(modal.NewUnitCounter);
                    pageCounterList.Add(modal.NewUnitCounter);
                }
                pageCounterView.Refresh();
            }
        }

        private void EditCounter_btn_Click(object sender, RoutedEventArgs e)
        {
            PageCounterViewModel counterVm = (sender as FrameworkElement).DataContext as PageCounterViewModel;
            if (counterVm == null) return;
            var modal = new counter_add_modal();
            var clone = counterVm.DeepClone();
            modal.DataContext = clone;
            ModalResult result = ModalForm.ShowModal(modal, "Edit Page Counter", ModalButtons.SaveCancelDelete);
            if (result == ModalResult.Save)
            {
                clone.DeepCopyTo(counterVm);
                _ = PrintersPageComputationHelper.AddUpdatePageCounter(counterVm);
                pageCounterView.Refresh();
            }
            else if (result == ModalResult.Delete)
            {
                _ = PrintersPageComputationHelper.DeletePageCounter(counterVm);
                pageCounterList.Remove(counterVm);
                pageCounterView.Refresh();
            }
        }

        private void OpenPageCountFile_btn_Click(object sender, RoutedEventArgs e)
        {
            PageCounterViewModel counterVm = (sender as FrameworkElement).DataContext as PageCounterViewModel;
            string file = FileHelper.GetFile(counterVm.FileAttachment, PrintersPageComputationHelper.COUNTER_FILE_DIRECTORY);
            if (System.IO.File.Exists(file))
                FileHelper.Open(file);
            else
                System.Windows.MessageBox.Show("ERROR: File not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private PageCounterViewModel computationGroup_SetCounter()
        {
            PageCounterViewModel selectedCounter = counterDataGrid.SelectedItem as PageCounterViewModel;
            return selectedCounter;
        }

        private void computationGroup_SetCounter_1(object sender, Templates.SetCounterEventArgs e)
        {
            selectPeriodPopup.PlacementTarget = sender as UIElement;
            selectPeriodDatagrid.ItemsSource = pageCounterView.OfType<PageCounterViewModel>().ToList();
            selectPeriodPopup.IsOpen = true;
            if (selectPeriodPopup.IsSubmitted)
            {
                e.SetCounter(selectPeriodDatagrid.SelectedItem as PageCounterViewModel);
            }
        }

        private void PrintReport_btn_Click(object sender, RoutedEventArgs e)
        {
            ComputationGroupViewModel computationVm = (sender as FrameworkElement).DataContext as ComputationGroupViewModel;
            Reports.ReportsHelper.GeneratePageCountComputationReport(computationVm);
        }

        private void DeleteReport_btn_Click(object sender, RoutedEventArgs e)
        {
            ComputationGroupViewModel computationVm = (sender as FrameworkElement).DataContext as ComputationGroupViewModel;
            string msg = string.Format("Do you want do DELETE the following Report?\n\nPeriod: {0:MM/dd/yyyy} - {1:MM/dd/yyyy}\nFor the month of: {2} {3}", computationVm.FirstCounter.Date, computationVm.LastCounter.Date, computationVm.ConsumptionMonth.ToEnum<MonthNames>().ToString(), computationVm.ConsumptionYear);
            if (System.Windows.MessageBox.Show(msg, "Delete Report", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _ = PrintersPageComputationHelper.DeleteComputationReport(computationVm);
                computationList.Remove(computationVm);
                computationView.Refresh();
            }
        }

        private void computationGroup_SaveReport(object sender, Templates.SaveReportEventArgs e)
        {
            //check if complete
            if (e.Computation.Contract == null)
            {
                System.Windows.MessageBox.Show("Save Report Failed.\n\nReason: No Contract Info.", "Save Report", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (e.Computation.FirstCounter == null)
            {
                System.Windows.MessageBox.Show("Save Report Failed.\n\nReason: No First Page Count Period Info.", "Save Report", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (e.Computation.LastCounter == null)
            {
                System.Windows.MessageBox.Show("Save Report Failed.\n\nReason: No Last Page Count Info.", "Save Report", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var modal = new page_count_report_save_modal();
            var clone = e.Computation.DeepClone();
            clone.ConsumptionMonth = clone.FirstCounter.Date.Month;
            clone.ConsumptionYear = clone.FirstCounter.Date.Year;
            clone.CreatedDate = DateTime.Now;
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, "Save Report", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(e.Computation);
                _ = PrintersPageComputationHelper.SaveComputationReport(e.Computation);
                computationList.Add(e.Computation);
                computationView.Refresh();
                System.Windows.MessageBox.Show("Report Saved Successfully", "Save Report", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditComputation_btn_Click(object sender, RoutedEventArgs e)
        {
            var computation = (sender as FrameworkElement).DataContext as ComputationGroupViewModel;
            var modal = new page_count_report_save_modal();
            var clone = computation.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, "Edit Computation", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(computation);
                _ = PrintersPageComputationHelper.SaveComputationReport(computation);
                computationView.Refresh();
            }
        }

        private bool InvoiceCompany_SearchText(object item, string searchText)
        {
            string keyword = searchText.Trim();
            var company = item as CompanyViewModel;
            return company.CompanyName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private void SelectInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (selectInvoice.SelectCompany == null) return;
            selectInvoicePopup.PlacementTarget = sender as UIElement;
            var list = computationList.Where(i => i.Contract.Company.Id == selectInvoice.SelectCompany.Id && i.ConsumptionMonth == selectInvoice.ConsumptionMonth && i.ConsumptionYear == selectInvoice.ConsumptionYear)
                                    .OrderByDescending(i => i.ConsumptionDate);
            var data = new
            {
                Totals = new TotalGroup() { TotalGrandTotal = list.Sum(i => i.GrandTotal) },
                List = list
            };
            selectInvoicePopup.DataContext = data;
            selectInvoicePopup.IsOpen = true;
            selectInvoicePopup.DataContext = null;
        }

        private void OpenJustificationFile_Click(object sender, RoutedEventArgs e)
        {
            ComputationGroupViewModel computation = (sender as FrameworkElement).DataContext as ComputationGroupViewModel;
            string file = FileHelper.GetFile(computation.JustificationFile, PrintersPageComputationHelper.JUSTIFICATION_FILE_DIRECTORY);
            if (System.IO.File.Exists(file))
                FileHelper.Open(file);
            else
                System.Windows.MessageBox.Show("ERROR: File not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
