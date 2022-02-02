using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace Egate_Printers_Page_Computation.Reports
{
    public partial class report_form : Form
    {
        private ReportViewer reportViewer;
        private Action<SubreportProcessingEventArgs> subreportProcessingCallback;

        public report_form(string reportFileName)
        {
            InitializeComponent();

            reportViewer = new ReportViewer();
            this.Controls.Add(reportViewer);

            reportViewer.Dock = DockStyle.Fill;
            reportViewer.PageCountMode = PageCountMode.Actual;
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
            reportViewer.ShowBackButton = false;
            reportViewer.ShowCredentialPrompts = false;
            reportViewer.ShowDocumentMapButton = false;
            reportViewer.ShowFindControls = false;
            reportViewer.ShowRefreshButton = false;
            reportViewer.ShowStopButton = false;

            reportViewer.LocalReport.EnableExternalImages = true;
            reportViewer.LocalReport.EnableHyperlinks = true;
            reportViewer.LocalReport.ReportEmbeddedResource = "Egate_Printers_Page_Computation.Reports.ReportFiles."+reportFileName+".rdlc";
            reportViewer.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
            reportViewer.LocalReport.DataSources.Clear();
        }

        public report_form(string reportFileName, Action<SubreportProcessingEventArgs> subreportCallback) : this(reportFileName)
        {
            this.subreportProcessingCallback = subreportCallback;
        }

        private void report_form_Load(object sender, EventArgs e)
        {
            reportViewer.RefreshReport();
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            subreportProcessingCallback?.Invoke(e);
        }

        public void AddDataSet(string datasourceName, object datasource)
        {
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(datasourceName, datasource));
        }

        public static void ShowReport(string reportFileName, string datasourceName, object datasource)
        {
            report_form.ShowReport(reportFileName, datasourceName, datasource, null);
        }

        public static void ShowReport(string reportFileName, string datasourceName, object datasource, Action<SubreportProcessingEventArgs> subreportCallback)
        {
            report_form.ShowReport(reportFileName, new Dictionary<string, object>() { { datasourceName, datasource } }, subreportCallback);
        }

        public static void ShowReport(string reportFileName, Dictionary<string, object> datasourceCollection, Action<SubreportProcessingEventArgs> subreportCallback)
        {
            var frm = new report_form(reportFileName, subreportCallback);
            foreach (var i in datasourceCollection)
                frm.AddDataSet(i.Key, i.Value);
            frm.ShowDialog();
        }
    }
}
