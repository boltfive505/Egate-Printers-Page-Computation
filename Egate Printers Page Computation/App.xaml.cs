using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Egate_Printers_Page_Computation.Objects.CollectionSchedule;

namespace Egate_Printers_Page_Computation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeErrorHandling();

            InitializeDatabase();
            var window = new MainWindow();
            window.Show();
        }

        private void InitializeDatabase()
        {
            List<Task> initializeTask = new List<Task>();
            initializeTask.Add(InitializeDatabaseAsync<PrintersPageComputation.Model.PrintersPageComputationModel>(cx => cx.Initialize()));
            initializeTask.Add(InitializeDatabaseAsync<CollectionSchedule.Model.CollectionScheduleModel>(cx => cx.Initialize()));
            initializeTask.Add(InitializeDatabaseAsync<Tutorials.Model.TutorialsModel>(cx => cx.Initialize()));
            Task.WaitAll(initializeTask.ToArray());
        }

        private static Task InitializeDatabaseAsync<T>(Action<T> initialize) where T : System.Data.Entity.DbContext, new()
        {
            Task t = Task.Run(() =>
            {
                using (var context = new T())
                {
                    initialize(context);
                }
            });
            return t;
        }

        private void InitializeErrorHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logs.WriteException(e.Exception);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logs.WriteException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logs.WriteException(e.ExceptionObject as Exception);
        }
    }
}
