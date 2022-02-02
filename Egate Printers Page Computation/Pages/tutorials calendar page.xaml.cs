using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Printers_Page_Computation.Objects.Tutorials;
using Egate_Printers_Page_Computation.Objects.Calendar;
using Egate_Printers_Page_Computation.Helpers;
using Egate_Printers_Page_Computation.SubModals.Tutorials;
using CustomControls.Modal;

namespace Egate_Printers_Page_Computation.Pages
{
    /// <summary>
    /// Interaction logic for tutorials_calendar_page.xaml
    /// </summary>
    public partial class tutorials_calendar_page : Page
    {
        public static readonly RoutedEvent SelectJobScheduleEvent = EventManager.RegisterRoutedEvent(nameof(SelectJobSchedule), RoutingStrategy.Bubble, typeof(SelectJobScheduleEventHandler), typeof(tutorials_calendar_page));
        public event SelectJobScheduleEventHandler SelectJobSchedule
        {
            add { AddHandler(SelectJobScheduleEvent, value); }
            remove { RemoveHandler(SelectJobScheduleEvent, value); }
        }

        private ICollectionView periodView;
        private ICollectionView employeeView;
        private List<TutorialVideoViewModel> videoList = new List<TutorialVideoViewModel>();
        private List<PeriodCalendarDisplayCollection<TutorialVideoViewModel>> periodList = new List<PeriodCalendarDisplayCollection<TutorialVideoViewModel>>();
        private List<TutorialEmployeeViewModel> employeeList = new List<TutorialEmployeeViewModel>();

        private TutorialEmployeeViewModel selectedEmployee = null;

        public tutorials_calendar_page()
        {
            InitializeComponent();
            periodView = new CollectionViewSource() { Source = periodList }.View;
            calendar.ItemsSource = periodView;

            employeeView = new CollectionViewSource() { Source = employeeList }.View;
            employeeView.SortDescriptions.Add(new SortDescription("EmployeeName", ListSortDirection.Ascending));
            employeeDataGrid.ItemsSource = employeeView;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            employeeList.ForEach(i => i.VideoCount = videoList.Count(v => i.Id == v.EmployeeAssignedTo?.Id));
        }

        private void LoadData()
        {
            List<Task> dataTasks = new List<Task>();
            dataTasks.Add(LoadDataAsync(employeeList, TutorialsHelper.GetEmployeeListAsync(), employeeView));
            dataTasks.Add(LoadDataAsync(videoList, TutorialsHelper.GetVideoListAsync(), null, i => !i.IsPeriodNotApplicable && i.PeriodType != TutorialPeriodType.None));
            Task.WaitAll(dataTasks.ToArray());
        }

        private async Task LoadDataAsync<T>(List<T> list, Task<IEnumerable<T>> task, ICollectionView view, Func<T, bool> predicate = null)
        {
            list.Clear();
            var result = await task;
            if (predicate != null)
                result = result.Where(predicate);
            list.AddRange(result);
            Dispatcher.Invoke(new Action(() => view?.Refresh()), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void calendar_DisplayMonthChanged(object sender, EventArgs e)
        {
            RefreshPeriodCalendarDisplay();
        }

        private void RefreshPeriodCalendarDisplay()
        {
            periodList.Clear();
            if (selectedEmployee != null)
            {
                var list = videoList.Where(i => i.EmployeeAssignedTo.Id == selectedEmployee.Id);
                var periodCollection = PeriodCalendarHelper.GetPeriodListByDisplayMonth(list, calendar.DisplayMonth.Year, calendar.DisplayMonth.Month);
                periodList.AddRange(periodCollection);
            }
            periodView.Refresh();
        }

        private void calendar_DayClick(object sender, CustomMonthlyCalendar.DayClickEventArgs e)
        {
            var btn = e.OriginalSource as CustomMonthlyCalendar.MonthlyCalendarDayButton;
            if (btn.DataContext != null)
            {
                schedulePopup.DataContext = btn.DataContext;
                schedulePopup.PlacementTarget = btn;
                schedulePopup.IsOpen = true;
            }
        }

        private void PrintSchedule_btn_Click(object sender, RoutedEventArgs e)
        {
            //e.Handled = true;
            //PeriodCalendarDisplayCollection<TutorialVideoViewModel> periodCollection = (sender as FrameworkElement).DataContext as PeriodCalendarDisplayCollection<TutorialVideoViewModel>;
            //string xml = web_print.WebPrintHelper.GenerateXmlForScheduleList(periodCollection.Items.Select(i => i.Item));
            //string file = web_print.WebPrintHelper.CreatePathForPrintableXmlWithStyleSheet(xml, "schedule");
            //web_print.WebPrintHelper.Print(file, "Print Schedule");
        }

        private void AddEmployee_btn_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Employee";
            var employee = (sender as FrameworkElement).DataContext as TutorialEmployeeViewModel;
            if (employee == null)
            {
                isEdit = false;
                title = "Add Employee";
                employee = new TutorialEmployeeViewModel();
            }
            var modal = new employee_add_modal();
            var clone = employee.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                _ = TutorialsHelper.AddUpdateEmployee(employee);
                if (!isEdit)
                    employeeList.Add(employee);
                employeeView.Refresh();
            }
        }

        private void EmployeeDataGrid_row_Selected(object sender, RoutedEventArgs e)
        {
            selectedEmployee = (sender as FrameworkElement).DataContext as TutorialEmployeeViewModel;
            RefreshPeriodCalendarDisplay();
        }

        private void ScheduleItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TutorialVideoViewModel video = ((sender as FrameworkElement).DataContext as PeriodCalendarDisplay<TutorialVideoViewModel>).Item;
            RaiseEvent(new SelectJobScheduleEventArgs(SelectJobScheduleEvent, video));
        }
    }

    public class SelectJobScheduleEventArgs : RoutedEventArgs
    {
        public TutorialVideoViewModel Video { get; private set; }

        public SelectJobScheduleEventArgs(RoutedEvent routedEvent, TutorialVideoViewModel video) : base(routedEvent)
        {
            this.Video = video;
        }
    }

    public delegate void SelectJobScheduleEventHandler(object sender, SelectJobScheduleEventArgs e);
}
