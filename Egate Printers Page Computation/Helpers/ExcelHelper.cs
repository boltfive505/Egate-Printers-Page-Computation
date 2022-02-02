using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using Microsoft.Win32;
using System.IO;
using System.Collections;
using System.Globalization;
using Egate_Printers_Page_Computation.Objects.CollectionSchedule;

namespace Egate_Printers_Page_Computation.Helpers
{
    public static class ExcelHelper
    {
        public static void ExportCompanyList(IEnumerable<CollectionCompanyViewModel> list)
        {
            //prepare fields
            FieldExpressionCollection<CollectionCompanyViewModel> expressionList = new FieldExpressionCollection<CollectionCompanyViewModel>();
            expressionList.Add(i => i.CompanyName, "Company");
            expressionList.Add(i => i.ClientType, "Type");
            expressionList.Add(i => i.Location.LocationName, "Location");
            expressionList.Add(i => i.ContactPerson, "Contact Person");
            expressionList.Add(i => i.ContactNumber, "Contact #");
            expressionList.Add(i => i.Email, "Email");
            expressionList.Add(i => i.ScheduleNotes, "Schedule Notes"); 

            //prepare excel
            byte[] excelData = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("companies");

                    //create header
                    IRow headerRow = sheet.CreateRow(0);
                    for (int i = 0; i < expressionList.Count; i++)
                        headerRow.CreateCell(i).SetCellValue(expressionList[i].GetFieldName());
                    //create rows
                    int r = 1;
                    foreach (var i in list)
                    {
                        IRow row = sheet.CreateRow(r);
                        for (int a = 0; a < expressionList.Count; a++)
                            row.CreateCell(a).SetCellObjectValue(expressionList[a].GetValue(i));
                        r++;
                    }

                    workbook.Write(ms);
                    excelData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logs.WriteException(ex);
                System.Windows.MessageBox.Show("An error occured while processing the file.", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            SaveExcelData(excelData, "Save Collection Company List", string.Format("collection company_{0:yyyy-MM-dd}", DateTime.Now));
        }

        public static void ExportCompanySimpleList(IEnumerable<CollectionCompanyViewModel> list, string name)
        {
            //prepare fields
            FieldExpressionCollection<CollectionCompanyViewModel> expressionList = new FieldExpressionCollection<CollectionCompanyViewModel>();
            expressionList.Add(i => i.CompanyName, "Company");
            expressionList.Add(i => i.ClientType, "Type");
            expressionList.Add(i => i.Location.LocationName, "Location");
            expressionList.Add(i => i.ContactPerson, "Contact Person");
            expressionList.Add(i => i.ContactNumber, "Contact #");
            expressionList.Add(i => i.Email, "Email");

            //prepare excel
            byte[] excelData = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("companies");

                    //create header
                    IRow headerRow = sheet.CreateRow(0);
                    for (int i = 0; i < expressionList.Count; i++)
                        headerRow.CreateCell(i).SetCellValue(expressionList[i].GetFieldName());
                    //create rows
                    int r = 1;
                    foreach (var i in list)
                    {
                        IRow row = sheet.CreateRow(r);
                        for (int a = 0; a < expressionList.Count; a++)
                            row.CreateCell(a).SetCellObjectValue(expressionList[a].GetValue(i));
                        r++;
                    }

                    workbook.Write(ms);
                    excelData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logs.WriteException(ex);
                System.Windows.MessageBox.Show("An error occured while processing the file.", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            SaveExcelData(excelData, "Save Collection Company List", string.Format("{0}_{1}", name, DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        public static void ExportScheduleList(IEnumerable<CollectionScheduleViewModel> list)
        {
            //prepare fields
            FieldExpressionCollection<CollectionScheduleViewModel> expressionList = new FieldExpressionCollection<CollectionScheduleViewModel>();
            expressionList.Add(i => i.UpdatedDate, "Updated Date");
            expressionList.Add(i => i.ScheduleDate, i => !i.NotScheduled, "Scheduled");
            expressionList.Add(i => i.Company.CompanyName, "Company");
            expressionList.Add(i => i.Company.Location.LocationName, "Location");
            expressionList.Add(i => i.Company.ContactPerson, "Contact Person");
            expressionList.Add(i => i.Company.ContactNumber, "Contact #");
            expressionList.Add(i => i.ScheduleNotes, "Schedule Notes");
            expressionList.Add(i => i.UpdateByEmployee.EmployeeName, "Updated By");

            //prepare excel
            byte[] excelData = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("collection schedule");

                    //create header
                    IRow headerRow = sheet.CreateRow(0);
                    for (int i = 0; i < expressionList.Count; i++)
                        headerRow.CreateCell(i).SetCellValue(expressionList[i].GetFieldName());
                    //create rows
                    int r = 1;
                    foreach (var i in list)
                    {
                        IRow row = sheet.CreateRow(r);
                        for (int a = 0; a < expressionList.Count; a++)
                            row.CreateCell(a).SetCellObjectValue(expressionList[a].GetValue(i));
                        r++;
                    }

                    workbook.Write(ms);
                    excelData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logs.WriteException(ex);
                System.Windows.MessageBox.Show("An error occured while processing the file.", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            SaveExcelData(excelData, "Save Collection Schedule List", string.Format("collection schedule_{0:yyyy-MM-dd}", DateTime.Now));
        }

        public static void ExportCollectionList(IEnumerable<CollectionScheduleViewModel> list)
        {
            //prepare fields
            FieldExpressionCollection<CollectionScheduleViewModel> expressionList = new FieldExpressionCollection<CollectionScheduleViewModel>();
            expressionList.Add(i => i.CollectedDate, "Collected Date");
            expressionList.Add(i => i.Company.CompanyName, "Company");
            expressionList.Add(i => i.Company.Location.LocationName, "Location");
            expressionList.Add(i => i.CheckAmount, "Check Amount");
            expressionList.Add(i => i.CashAmount, "Cash Amount");
            expressionList.Add(i => i.ReceiptNumber, "OR No. / CR No.");
            expressionList.Add(i => i.InvoiceNumber, "Invoice No.");
            expressionList.Add(i => i.Bank.BankName, "Bank");
            expressionList.Add(i => i.CheckNumber, "Check No.");
            expressionList.Add(i => i.WhtAmount, "WHT");
            expressionList.Add(i => i.NetWhtAmount, "Net");
            expressionList.Add(i => i.UpdatedDate, "Updated Date");
            expressionList.Add(i => i.UpdateByEmployee.EmployeeName, "Updated By");
            expressionList.Add(i => i.CollectionNotes, "Notes");

            //prepare excel
            byte[] excelData = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("collection");

                    //create header
                    IRow headerRow = sheet.CreateRow(0);
                    for (int i = 0; i < expressionList.Count; i++)
                        headerRow.CreateCell(i).SetCellValue(expressionList[i].GetFieldName());
                    //create rows
                    int r = 1;
                    foreach (var i in list)
                    {
                        IRow row = sheet.CreateRow(r);
                        for (int a = 0; a < expressionList.Count; a++)
                            row.CreateCell(a).SetCellObjectValue(expressionList[a].GetValue(i));
                        r++;
                    }

                    workbook.Write(ms);
                    excelData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logs.WriteException(ex);
                System.Windows.MessageBox.Show("An error occured while processing the file.", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            SaveExcelData(excelData, "Save Collection  List", string.Format("collection_{0:yyyy-MM-dd}", DateTime.Now));
        }

        public static void ExportInvoiceList(IEnumerable<InvoiceViewModel> list, int invoiceYear, int invoiceMonth)
        {
            //prepare fields
            FieldExpressionCollection<InvoiceViewModel> expressionList = new FieldExpressionCollection<InvoiceViewModel>();
            expressionList.Add(i => i.Company.CompanyName, "Company");
            expressionList.Add(i => i.Amount, "Amount");
            expressionList.Add(i => i.InvoiceNumber, "Invoice No.");
            expressionList.Add(i => i.Notes, "Notes");
            expressionList.Add(i => i.Company.ClientType, "Type");
            expressionList.Add(i => i.Company.Location.LocationName, "Location");
            expressionList.Add(i => i.InvoiceDate, "Month", "MMMM yyyy");
            expressionList.Add(i => i.UpdatedDate, "Updates");

            string monthName = invoiceYear + "_" + (invoiceMonth == 0 ? "All Months" : new DateTime(1, invoiceMonth, 1).ToString("MMMM", CultureInfo.InvariantCulture));

            //prepare excel
            byte[] excelData = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet(monthName);

                    //create header
                    IRow headerRow = sheet.CreateRow(0);
                    for (int i = 0; i < expressionList.Count; i++)
                        headerRow.CreateCell(i).SetCellValue(expressionList[i].GetFieldName());
                    //create rows
                    int r = 1;
                    foreach (var i in list)
                    {
                        IRow row = sheet.CreateRow(r);
                        for (int a = 0; a < expressionList.Count; a++)
                            row.CreateCell(a).SetCellObjectValue(expressionList[a].GetValue(i));
                        r++;
                    }

                    workbook.Write(ms);
                    excelData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logs.WriteException(ex);
                System.Windows.MessageBox.Show("An error occured while processing the file.", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            string saveFileName = monthName + "_rental invoice";
            SaveExcelData(excelData, "Save Invoice List", saveFileName);
        }

        public static void ExportMissingInvoiceList(IEnumerable<InvoiceViewModel> list)
        {
            //prepare fields
            FieldExpressionCollection<InvoiceViewModel> expressionList = new FieldExpressionCollection<InvoiceViewModel>();
            expressionList.Add(i => i.Company.CompanyName, "Company");
            expressionList.Add(i => i.Notes, "Notes");
            expressionList.Add(i => i.ComparisonNotes, "Bill Notes");

            //prepare excel
            byte[] excelData = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("invoice");

                    //create header
                    IRow headerRow = sheet.CreateRow(0);
                    for (int i = 0; i < expressionList.Count; i++)
                        headerRow.CreateCell(i).SetCellValue(expressionList[i].GetFieldName());
                    //create rows
                    int r = 1;
                    foreach (var i in list)
                    {
                        IRow row = sheet.CreateRow(r);
                        for (int a = 0; a < expressionList.Count; a++)
                            row.CreateCell(a).SetCellObjectValue(expressionList[a].GetValue(i));
                        r++;
                    }

                    workbook.Write(ms);
                    excelData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logs.WriteException(ex);
                System.Windows.MessageBox.Show("An error occured while processing the file.", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            //string saveFileName = monthName + "_rental invoice";
            SaveExcelData(excelData, "Save Invoice List", "invoice missing list");
        }

        private static void SaveExcelData(byte[] excelData, string title, string fileName)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Title = title;
            save.Filter = "Excel File|*.xlsx";
            save.FileName = fileName;
            if (save.ShowDialog() == true)
            {
                try
                {
                    using (var fs = new FileStream(save.FileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(excelData, 0, excelData.Length);
                    }
                    FileHelper.Open(save.FileName);
                }
                catch (IOException ioEx)
                {
                    Logs.WriteException(ioEx);
                    System.Windows.MessageBox.Show(ioEx.Message);
                }
            }
        }

        private class FieldExpressionCollection<T> : List<FieldExpression<T>>
        {
            public void Add(Expression<Func<T, object>> expression)
            {
                this.Add(expression, null, null, null);
            }

            public void Add(Expression<Func<T, object>> expression, string fieldName)
            {
                this.Add(expression, null, fieldName, null);
            }

            public void Add(Expression<Func<T, object>> expression, Func<T, bool> predicate, string fieldName)
            {
                this.Add(expression, predicate, fieldName, null);
            }

            public void Add(Expression<Func<T, object>> expression, string fieldName, string formatString)
            {
                this.Add(expression, null, fieldName, formatString);
            }

            public void Add(Expression<Func<T, object>> expression, Func<T, bool> predicate, string fieldName, string formatString)
            {
                this.Add(new FieldExpression<T>(expression, predicate, fieldName, formatString));
            }
        }

        private class FieldExpression<T>
        {
            private Expression<Func<T, object>> _expression;
            private string _fieldName;
            private string _formatString;
            private Func<T, bool> _predicate;

            public FieldExpression(Expression<Func<T, object>> expression, Func<T, bool> predicate, string fieldName, string formatString)
            {
                this._expression = expression;
                this._predicate = predicate;
                this._fieldName = fieldName;
                this._formatString = formatString;
            }

            public string GetFieldName()
            {
                if (!string.IsNullOrEmpty(_fieldName))
                    return _fieldName;
                else
                {
                    if (_expression.Body is MemberExpression)
                    {
                        return ((MemberExpression)_expression.Body).Member.Name;
                    }
                    else
                    {
                        var op = ((UnaryExpression)_expression.Body).Operand;
                        return ((MemberExpression)op).Member.Name;
                    }
                }
            }

            public object GetValue(T parent)
            {
                try
                {
                    //check for condition
                    if (_predicate != null && !_predicate(parent))
                        return null;
                }
                catch (NullReferenceException)
                { }

                object value = null;
                try
                {
                    //get value from expression
                    var method = _expression.Compile();
                    value = method(parent);
                }
                catch (NullReferenceException)
                { }
                if (value == null) return null;
                Type type = value.GetType();
                if (Nullable.GetUnderlyingType(type) != null)
                    type = Nullable.GetUnderlyingType(type);
                if (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
                {
                    //if value is number value, do nothing, leave as is
                }
                else
                {
                    value = GetFormattedStringValue(value);
                }
                return value;
            }

            public string GetStringValue(T parent)
            {
                try
                {
                    //check for condition
                    if (_predicate != null && !_predicate(parent))
                        return null;
                }
                catch (NullReferenceException)
                { }

                object value = null;
                try
                {
                    //get value from expression
                    var method = _expression.Compile();
                    value = method(parent);
                }
                catch (NullReferenceException)
                { }

                if (value != null)
                {
                    value = GetFormattedStringValue(value);
                }
                return Convert.ToString(value);
            }

            private string GetFormattedStringValue(object value)
            {
                IFormattable formattable = value as IFormattable;
                if (formattable != null)
                {
                    string format = !string.IsNullOrEmpty(this._formatString) ? this._formatString : GetFormatStringForType(value.GetType());
                    return formattable.ToString(format, CultureInfo.InvariantCulture);
                }
                return Convert.ToString(value);
            }

            private static string GetFormatStringForType(Type type)
            {
                if (type != null)
                {
                    if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
                        return "#.#0";
                    else if (type == typeof(DateTime))
                        return "M/d/yyyy";
                    else if (type == typeof(TimeSpan))
                        return "c";
                }
                return "";
            }
        }
    }
}
