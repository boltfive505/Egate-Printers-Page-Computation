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
using Egate_Printers_Page_Computation.Objects;

namespace Egate_Printers_Page_Computation.Templates
{
    /// <summary>
    /// Interaction logic for page_count_computation.xaml
    /// </summary>
    public partial class page_count_computation : UserControl
    {
        public static readonly DependencyProperty ContractProperty = DependencyProperty.Register(nameof(Contract), typeof(ContractViewModel), typeof(page_count_computation), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnContractPropertyChanged)));
        public ContractViewModel Contract
        {
            get { return (ContractViewModel)GetValue(ContractProperty); }
            set { SetValue(ContractProperty, value); }
        }

        public static readonly DependencyProperty LastCounterProperty = DependencyProperty.Register(nameof(LastCounter), typeof(PageCounterViewModel), typeof(page_count_computation), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLastCounterPropertyChanged)));
        public PageCounterViewModel LastCounter
        {
            get { return (PageCounterViewModel)GetValue(LastCounterProperty); }
            set { SetValue(LastCounterProperty, value); }
        }

        public static readonly DependencyProperty FirstCounterProperty = DependencyProperty.Register(nameof(FirstCounter), typeof(PageCounterViewModel), typeof(page_count_computation), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFirstCounterPropertyChanged)));
        public PageCounterViewModel FirstCounter
        {
            get { return (PageCounterViewModel)GetValue(FirstCounterProperty); }
            set { SetValue(FirstCounterProperty, value); }
        }

        public static readonly DependencyProperty ComputationsProperty = DependencyProperty.Register(nameof(Computations), typeof(ComputationGroupViewModel), typeof(page_count_computation));
        public ComputationGroupViewModel Computations
        {
            get { return (ComputationGroupViewModel)GetValue(ComputationsProperty); }
            set { SetValue(ComputationsProperty, value); }
        }

        public event EventHandler<SetCounterEventArgs> SetCounter;
        public event EventHandler<SaveReportEventArgs> SaveReport;

        public page_count_computation()
        {
            InitializeComponent();
            Computations = new ComputationGroupViewModel();
        }

        private void PrintReport_btn_Click(object sender, RoutedEventArgs e)
        {
            Reports.ReportsHelper.GeneratePageCountComputationReport(this.Computations);
        }

        private void SetLatestPeriod_btn_Click(object sender, RoutedEventArgs e)
        {
            SetCounterEventArgs args = new SetCounterEventArgs();
            SetCounter?.Invoke(sender, args);
            if (args.HasSelected)
                LastCounter = args.Counter;
        }

        private void SetFirstPeriod_btn_Click(object sender, RoutedEventArgs e)
        {
            SetCounterEventArgs args = new SetCounterEventArgs();
            SetCounter?.Invoke(sender, args);
            if (args.HasSelected)
                FirstCounter = args.Counter;
        }

        private void SwapPeriods_btn_Click(object sender, RoutedEventArgs e)
        {
            var temp = LastCounter;
            LastCounter = FirstCounter;
            FirstCounter = temp;
        }

        private static void OnLastCounterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as page_count_computation).Computations.LastCounter = e.NewValue as PageCounterViewModel;
        }

        private static void OnFirstCounterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as page_count_computation).Computations.FirstCounter = e.NewValue as PageCounterViewModel;
        }

        private static void OnContractPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as page_count_computation).Computations.Contract = e.NewValue as ContractViewModel;
            //if contract changed, clear periods
            (sender as page_count_computation).Computations.LastCounter = null;
            (sender as page_count_computation).Computations.FirstCounter = null;
        }

        private void SaveReport_btn_Click(object sender, RoutedEventArgs e)
        {
            SaveReport?.Invoke(sender, new SaveReportEventArgs(this.Computations.DeepClone()));
        }
    }

    public class SetCounterEventArgs : EventArgs
    {
        public PageCounterViewModel Counter { get; private set; }
        public bool HasSelected { get; private set; }

        public void SetCounter(PageCounterViewModel counter)
        {
            this.Counter = counter;
            HasSelected = true;
        }
    }

    public class SaveReportEventArgs : EventArgs
    {
        public ComputationGroupViewModel Computation { get; private set; }

        public SaveReportEventArgs(ComputationGroupViewModel computation)
        {
            this.Computation = computation;
        }
    }
}
