using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Egate_Printers_Page_Computation.Objects.CollectionSchedule;

namespace Egate_Printers_Page_Computation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty PageTabNameProperty = DependencyProperty.RegisterAttached("PageTabName", typeof(PageTab), typeof(MainWindow), new PropertyMetadata(PageTab.None));
        public static readonly RoutedEvent ChangeTabEvent = EventManager.RegisterRoutedEvent("ChangeTab", RoutingStrategy.Bubble, typeof(PageTabEventHandler), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
        }

        public static PageTab GetPageTabName(DependencyObject obj)
        {
            return (PageTab)obj.GetValue(PageTabNameProperty);
        }

        public static void SetPageTabName(DependencyObject obj, PageTab value)
        {
            obj.SetValue(PageTabNameProperty, value);
        }

        public static void AddChangeTabHandler(DependencyObject obj, PageTabEventHandler handler)
        {
            UIElement element = obj as UIElement;
            element.AddHandler(MainWindow.ChangeTabEvent, handler);
        }

        public static void RemoveChangeTabHandler(DependencyObject obj, PageTabEventHandler handler)
        {
            UIElement element = obj as UIElement;
            element.RemoveHandler(MainWindow.ChangeTabEvent, handler);
        }

        private void tabControl_SelectJobSchedule(object sender, Pages.SelectJobScheduleEventArgs e)
        {
            tabControl.SelectedItem = tutorialsTab;
            tutorialsPage.SelectSchedule(e.Video);
        }

        public static void ChangeTab(object sender, PageTab pageTab)
        {
            MainWindow.ChangeTab(sender, pageTab, null);
        }

        public static void ChangeTab(object sender, PageTab pageTab, Action<Page> pageAction)
        {
            UIElement element = sender as UIElement;
            if (element != null)
                element.RaiseEvent(new PageTabEventArgs(MainWindow.ChangeTabEvent, pageTab, pageAction));
        }

        private void tabControl_ChangeTab(object sender, PageTabEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            TabItem tabPage = tabControl.Items.OfType<TabItem>().FirstOrDefault(i => MainWindow.GetPageTabName(i) == e.PageTab);
            if (tabPage != null)
            {
                tabControl.SelectedItem = tabPage;
                //invoke page action
                Page page = (tabPage.Content as Frame).Content as Page; //get page from content
                e.PageAction?.Invoke(page);
            }
        }
    }

    public class PageTabEventArgs : RoutedEventArgs
    {
        public PageTab PageTab { get; set; }
        public Action<Page> PageAction { get; set; }

        public PageTabEventArgs(RoutedEvent routedEvent, PageTab pageTab, Action<Page> pageAction) : base(routedEvent)
        {
            this.PageTab = pageTab;
            this.PageAction = pageAction;
        }
    }

    public delegate void PageTabEventHandler(object sender, PageTabEventArgs e);

    public enum PageTab
    {
        None,
        PageCounter,
        Collection,
        Collection_2307,
        Invoice,
        ToDoList,
        Setup,
        Tutorials,
        Calendar
    }
}
