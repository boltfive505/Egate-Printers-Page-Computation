using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Entity;
using CollectionSchedule.Model;
using Egate_Printers_Page_Computation.Objects.CollectionSchedule;
using System.Globalization;

namespace Egate_Printers_Page_Computation.Helpers
{
    public static class CollectionScheduleHelper
    {
        public const string FILE2307_DIRECTORY = @".\uploads\2307 files";
        public const string FILE_INVOICE_DIRECTORY = @".\uploads\invoice files";

        private static string ProcessFile(string oldFile, string newFile, string subdirectory)
        {
            return ProcessFile(oldFile, newFile, subdirectory, null, FileHelper.FileNameRenameMode.NoRename);
        }

        private static string ProcessFile(string oldFile, string newFile, string subdirectory, string rename, FileHelper.FileNameRenameMode renameMode = FileHelper.FileNameRenameMode.FullRename)
        {
            //check if old and new file is same
            if (oldFile == newFile)
                return oldFile;
            else
            {
                //upload new file, if exist
                if (File.Exists(newFile))
                    newFile = FileHelper.Upload(newFile, subdirectory, rename, renameMode);
                else
                    newFile = string.Empty;
                //delete old file, if exist
                string oldFile2 = FileHelper.GetFile(oldFile, subdirectory);
                if (File.Exists(oldFile2))
                    File.Delete(oldFile2);
                return Path.GetFileName(newFile);
            }
        }

        private static void DeleteFile(string oldFile, string subdirectory)
        {
            //delete old file, if exist
            string oldFile2 = FileHelper.GetFile(oldFile, subdirectory);
            if (File.Exists(oldFile2))
                File.Delete(oldFile2);
        }

        public static async Task<IEnumerable<CollectionCompanyViewModel>> GetCompanyListAsync()
        {
            using (var context = new CollectionScheduleModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from comp in context.company
                            join loc in context.location on comp.LocationId equals loc.Id into tempLoc
                            from loc_comp in tempLoc.DefaultIfEmpty()
                            orderby comp.CompanyName ascending
                            select new { Company = comp, Location = loc_comp };
                var list = await query.ToListAsync();
                return list.Select(i => new CollectionCompanyViewModel(i.Company, i.Location));
            }
        }

        public static async Task GetCompanyFollowupAllList(List<CollectionCompanyViewModel> list)
        {
            using (var context = new CollectionScheduleModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var scheduleQuery = from coll in context.collection
                                    join comp in context.company on coll.CompanyId equals comp.Id
                                    select new { Collection = coll, Company = comp };
                var invoiceQuery = from inv in context.invoice
                                   join comp in context.company on inv.CompanyId equals comp.Id
                                   select new { Invoice = inv, Company = comp };
                var scheduleList = await scheduleQuery.ToListAsync().ContinueWith(t => t.Result.Select(i => new CollectionScheduleViewModel(i.Collection, null, i.Company, null, null, null)));
                var invoiceList = await invoiceQuery.ToListAsync().ContinueWith(t => t.Result.Select(i => new InvoiceViewModel(i.Invoice, i.Company, null)));
                foreach (var l in list)
                {
                    l.HasFollowupCollection = scheduleList.Where(i => i.Company.Id == l.Id).Any(i => !i.IsCollected && !i.NotScheduled && i.ScheduleDate != null && i.ScheduleNotesType != ScheduleNotesType.NotScheduled);
                    l.HasFollowup2307 = scheduleList.Where(i => i.Company.Id == l.Id && i.Company.Is2307).Any(i => i.CollectedDate != null && (string.IsNullOrEmpty(i.File2307Attachment) || (i.WhtAmount ?? 0) == 0));
                    l.InvoiceNumberMissingList = invoiceList.Where(i => i.Company.Id == l.Id).Where(i => !string.IsNullOrEmpty(i.InvoiceNumber) && (i.Amount ?? 0) != 0 && string.IsNullOrEmpty(i.FileAttachment)).Select(i => i.InvoiceNumber).OrderBy(x => x).ToList();
                }
            }
        }

        public static async Task<IEnumerable<CollectionLocationViewModel>> GetLocationListAsync()
        {
            using (var context = new CollectionScheduleModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from loc in context.location
                            orderby loc.LocationName ascending
                            select loc;
                var list = await query.ToListAsync();
                return list.Select(i => new CollectionLocationViewModel(i));
            }
        }

        public static async Task<IEnumerable<CollectionBankViewModel>> GetBankListAsync()
        {
            using (var context = new CollectionScheduleModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from bk in context.bank
                            orderby bk.BankName ascending
                            select bk;
                var list = await query.ToListAsync();
                return list.Select(i => new CollectionBankViewModel(i));
            }
        }

        public static async Task<IEnumerable<CollectionScheduleViewModel>> GetScheduleListAsync()
        {
            using (var context = new CollectionScheduleModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from coll in context.collection
                            join bk in context.bank on coll.BankId equals bk.Id into tempBk
                            from bk_coll in tempBk.DefaultIfEmpty()
                            join comp in context.company on coll.CompanyId equals comp.Id
                            join loc in context.location on comp.LocationId equals loc.Id into tempLoc
                            from loc_comp in tempLoc.DefaultIfEmpty()
                            join upEmp in context.employee on coll.UpdatedByEmployeeId equals upEmp.Id into tempUpEmp
                            from coll_upEmp in tempUpEmp.DefaultIfEmpty()
                            join collbyEmp in context.employee on coll.CollectedByEmployeeId equals collbyEmp.Id into tempCollbyEmp
                            from coll_collbyEmp in tempCollbyEmp.DefaultIfEmpty()
                            select new
                            {
                                Collection = coll,
                                Bank = bk_coll,
                                Company = comp,
                                Location = loc_comp,
                                UpdatedByEmployee = coll_upEmp,
                                CollectedByEmployee = coll_collbyEmp
                            };
                var list = await query.ToListAsync();
                return list.Select(i => new CollectionScheduleViewModel(i.Collection, i.Bank, i.Company, i.Location, i.UpdatedByEmployee, i.CollectedByEmployee));
            }
        }

        public static async Task<IEnumerable<CollectionEmployeeViewModel>> GetEmployeeListAsync(bool includeInactive = true)
        {
            using (var context = new CollectionScheduleModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from emp in context.employee
                            where includeInactive ? true : emp.IsActive == 1
                            orderby emp.EmployeeName ascending
                            select emp;
                var list = await query.ToListAsync();
                return list.Select(i => new CollectionEmployeeViewModel(i));
            }
        }

        public static async Task<IEnumerable<InvoiceViewModel>> GetInvoiceListAsync()
        {
            using (var context = new CollectionScheduleModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from inv in context.invoice
                            join comp in context.company on inv.CompanyId equals comp.Id
                            join loc in context.location on comp.LocationId equals loc.Id into tempLoc
                            from loc_comp in tempLoc.DefaultIfEmpty()
                            select new { Invoice = inv, Company = comp, Location = loc_comp };
                var list = await query.ToListAsync();
                return list.Select(i => new InvoiceViewModel(i.Invoice, i.Company, i.Location));
            }
        }

        public static async Task AddScheduleAsync(CollectionScheduleViewModel scheduleVm)
        {
            using (var context = new CollectionScheduleModel())
            {
                var collection = await context.collection.FirstOrDefaultAsync(i => i.Id == scheduleVm.Id);
                if (collection == null)
                {
                    collection = new collection();
                    context.collection.Add(collection);
                }
                collection.CompanyId = scheduleVm.Company.Id;
                collection.ScheduleDate = scheduleVm.ScheduleDate.ToUnixLong();
                collection.UpdatedDate = scheduleVm.UpdatedDate.ToUnixLong();
                collection.UpdatedByEmployeeId = scheduleVm.UpdateByEmployee?.Id;
                collection.ReceiptNumber = scheduleVm.ReceiptNumber;
                collection.InvoiceNumber = scheduleVm.InvoiceNumber;
                collection.BankId = scheduleVm.Bank?.Id;
                collection.CheckNumber = scheduleVm.CheckNumber;
                collection.CheckAmount = scheduleVm.CheckAmount;
                collection.CashAmount = scheduleVm.CashAmount;
                collection.CollectedDate = scheduleVm.CollectedDate.ToUnixLong();
                collection.CollectedByEmployeeId = scheduleVm.CollectedByEmployee?.Id;
                collection.NotScheduled = scheduleVm.NotScheduled.ToLong();
                collection.ScheduleNotesType = scheduleVm.ScheduleNotesType.ToString();
                collection.ScheduleNotes = scheduleVm.ScheduleNotes;
                collection.CollectionNotes = scheduleVm.CollectionNotes;
                collection.File2307Attachment = ProcessFile(collection.File2307Attachment, scheduleVm.File2307Attachment, FILE2307_DIRECTORY);
                collection.WhtAmount = scheduleVm.WhtAmount;

                await context.SaveChangesAsync();
                scheduleVm.Id = (int)collection.Id;
                scheduleVm.File2307Attachment = collection.File2307Attachment;
            }
        }

        public static async Task DeleteScheduleAsync(CollectionScheduleViewModel scheduleVm)
        {
            using (var context = new CollectionScheduleModel())
            {
                var collection = await context.collection.FirstOrDefaultAsync(i => i.Id == scheduleVm.Id);
                if (collection != null)
                {
                    context.collection.Remove(collection);
                    await context.SaveChangesAsync();
                    DeleteFile(collection.File2307Attachment, FILE2307_DIRECTORY);
                }
            }
        }

        public static async Task DeleteScheduleMultipleAsync(IEnumerable<CollectionScheduleViewModel> scheduleList)
        {
            using (var context = new CollectionScheduleModel())
            {
                foreach (var s in scheduleList)
                {
                    var collection = await context.collection.FirstOrDefaultAsync(i => i.Id == s.Id);
                    if (collection != null)
                        context.collection.Remove(collection);
                }
                await context.SaveChangesAsync();
                //delete files
                foreach (var s in scheduleList)
                {
                    DeleteFile(s.File2307Attachment, FILE2307_DIRECTORY);
                }
            }
        }

        public static async Task AddCompanyAsync(CollectionCompanyViewModel companyVm)
        {
            using (var context = new CollectionScheduleModel())
            {
                var company = await context.company.FirstOrDefaultAsync(i => i.Id == companyVm.Id);
                if (company == null)
                {
                    company = new company();
                    context.company.Add(company);
                }
                company.CompanyName = companyVm.CompanyName;
                company.LocationId = companyVm.Location?.Id;
                company.ContactPerson = companyVm.ContactPerson;
                company.ContactNumber = companyVm.ContactNumber;
                company.Description = companyVm.Description;
                company.IsActive = companyVm.IsActive.ToLong();
                company.OverdueNotes = companyVm.OverdueNotes;
                company.ScheduleNotes = companyVm.ScheduleNotes;
                company.Email = companyVm.Email;
                company.Address = companyVm.Address;
                company.PhoneNumber = companyVm.PhoneNumber;
                company.TinNumber = companyVm.TinNumber;
                company.Is2307 = companyVm.Is2307.ToLong();
                company.IsBankTransfer = companyVm.IsBankTransfer.ToLong();
                company.ToDoNotes = companyVm.ToDoNotes;
                company.ClientType = companyVm.ClientType.ToString();

                await context.SaveChangesAsync();
                companyVm.Id = (int)company.Id;
            }
        }

        public static async Task AddLocationAsync(CollectionLocationViewModel locationVm)
        {
            using (var context = new CollectionScheduleModel())
            {
                var location = await context.location.FirstOrDefaultAsync(i => i.Id == locationVm.Id);
                if (location == null)
                {
                    location = new location();
                    context.location.Add(location);
                }
                location.LocationName = locationVm.LocationName;

                await context.SaveChangesAsync();
                locationVm.Id = (int)location.Id;
            }
        }

        public static async Task AddBankAsync(CollectionBankViewModel bankVm)
        {
            using (var context = new CollectionScheduleModel())
            {
                var bank = await context.bank.FirstOrDefaultAsync(i => i.Id == bankVm.Id);
                if (bank == null)
                {
                    bank = new bank();
                    context.bank.Add(bank);
                }
                bank.BankName = bankVm.BankName;

                await context.SaveChangesAsync();
                bankVm.Id = (int)bank.Id;
            }
        }

        public static async Task AddEmployeeAsync(CollectionEmployeeViewModel employeeVm)
        {
            using (var context = new CollectionScheduleModel())
            {
                var employee = await context.employee.FirstOrDefaultAsync(i => i.Id == employeeVm.Id);
                if (employee == null)
                {
                    employee = new employee();
                    context.employee.Add(employee);
                }
                employee.EmployeeName = employeeVm.EmployeeName;
                employee.Description = employeeVm.Description;
                employee.IsActive = employeeVm.IsActive.ToLong();

                await context.SaveChangesAsync();
                employeeVm.Id = (int)employee.Id;
            }
        }

        public static async Task GetScheduledCountAsync(List<CollectionCompanyViewModel> companyList, List<CollectionScheduleViewModel> scheduleList)
        {
            var tasks = companyList.Select(l => Task.Run(() =>
            {
                l.ScheduleCount = scheduleList.Count(i => i.Company.Id == l.Id && i.CollectedDate == null);
                if (l.ScheduleCount == 0) l.ScheduleCount = null;
            }));
            await Task.WhenAll(tasks);
        }

        public static async Task GetCompanyFollowUpAsync(List<CollectionCompanyViewModel> companyList, List<CollectionScheduleViewModel> scheduleList)
        {
            var tasks = companyList.Select(l => Task.Run(() =>
            {
                DateTime now = DateTime.Now;
                DateTime lastDayOfWeek = new DateTime(now.Year, now.Month, now.Day + ((int)DayOfWeek.Saturday - (int)now.DayOfWeek));
                l.HasFollowUp = scheduleList.Where(i => i.Company.Id == l.Id).Any(i => !i.IsCollected && i.UpdatedDate <= lastDayOfWeek);
            }));
            await Task.WhenAll(tasks);
        }

        public static async Task GetCompanyCurrentScheduleAsync(List<CollectionCompanyViewModel> companyList, List<CollectionScheduleViewModel> scheduleList)
        {
            var tasks = companyList.Select(l => Task.Run(() =>
            {
                var sched = scheduleList.Where(i => !i.IsCollected)
                                        .OrderByDescending(i => i.UpdatedDate)
                                        .FirstOrDefault(i => i.Company.Id == l.Id);
                l.CurrentScheduledCollection = sched;
            }));
            await Task.WhenAll(tasks);
        }

        public static async Task GetCompanyLatestCollectionAsync(List<CollectionCompanyViewModel> companyList, List<CollectionScheduleViewModel> scheduleList)
        {
            var tasks = companyList.Select(l => Task.Run(() =>
            {
                var sched = scheduleList.Where(i => i.IsCollected)
                                        .OrderByDescending(i => i.CollectedDate)
                                        .FirstOrDefault(i => i.Company.Id == l.Id);
                l.LatestCollected = sched;
            }));
            await Task.WhenAll(tasks);
        }

        public static async Task AddInvoiceAsync(InvoiceViewModel invoiceVm)
        {
            using (var context = new CollectionScheduleModel())
            {
                var invoice = await context.invoice.FirstOrDefaultAsync(i => i.Id == invoiceVm.Id);
                if (invoice == null)
                {
                    invoice = new invoice();
                    context.invoice.Add(invoice);
                }
                invoice.CompanyId = invoiceVm.Company.Id;
                invoice.InvoiceMonth = invoiceVm.InvoiceMonth;
                invoice.InvoiceYear = invoiceVm.InvoiceYear;
                invoice.InvoiceNumber = invoiceVm.InvoiceNumber;
                invoice.Amount = invoiceVm.Amount;
                string invoiceRename = string.Format("{0}_{1}_{2}", invoiceVm.InvoiceNumber, new DateTime(invoiceVm.InvoiceYear.Value, invoiceVm.InvoiceMonth.Value, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture), invoiceVm.Company.CompanyName);
                invoice.FileAttachment = ProcessFile(invoice.FileAttachment, invoiceVm.FileAttachment, FILE_INVOICE_DIRECTORY, invoiceRename);
                invoice.Notes = invoiceVm.Notes;
                invoice.ComparisonNotes = invoiceVm.ComparisonNotes;
                invoice.UpdatedDate = invoiceVm.UpdatedDate.ToUnixLong();

                await context.SaveChangesAsync();
                invoiceVm.Id = (int)invoice.Id;
                invoiceVm.FileAttachment = invoice.FileAttachment;
            }
        }

        public static async Task UpdateInvoiceComparisonNotesAsync(InvoiceViewModel invoiceVm)
        {
            using (var context = new CollectionScheduleModel())
            {
                var invoice = await context.invoice.FirstOrDefaultAsync(i => i.Id == invoiceVm.Id);
                if (invoice != null)
                {
                    invoice.ComparisonNotes = invoiceVm.ComparisonNotes;
                    await context.SaveChangesAsync();
                }
            }
        }

        public static bool CheckDuplicateInvoiceNumber(string invoiceNumber)
        {
            invoiceNumber = invoiceNumber.Trim();
            using (var context = new CollectionScheduleModel())
            {
                var query = from inv in context.invoice
                            where inv.InvoiceNumber == invoiceNumber
                            select 1;
                return query.ToList().Count > 0;
            }
        }
    }
}
