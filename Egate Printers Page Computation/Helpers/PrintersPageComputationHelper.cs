using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO;
using PrintersPageComputation.Model;
using Egate_Printers_Page_Computation.Objects;
using System.Globalization;

namespace Egate_Printers_Page_Computation.Helpers
{
    public static class PrintersPageComputationHelper
    {
        public const string COUNTER_FILE_DIRECTORY = @".\uploads\page count files";
        public const string JUSTIFICATION_FILE_DIRECTORY = @".\uploads\counter files";

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

        public static async Task<IEnumerable<CompanyViewModel>> GetCompanyListAsync()
        {
            using (var context = new PrintersPageComputationModel())
            {
                var query = from comp in context.company
                            select comp;
                var list = await query.ToListAsync();
                return list.Select(i => new CompanyViewModel(i));
            }
        }

        public static async Task<IEnumerable<ContractViewModel>> GetContractListAsync()
        {
            using (var context = new PrintersPageComputationModel())
            {
                var query = from cont in context.contract
                            join comp in context.company on cont.CompanyId equals comp.Id
                            select new { Contract = cont, Company = comp };
                var list = await query.ToListAsync();
                return list.Select(i => new ContractViewModel(i.Contract, i.Company));
            }
        }

        public static async Task<IEnumerable<PageCounterViewModel>> GetPageCounterList()
        {
            using (var context = new PrintersPageComputationModel())
            {
                var query = from cnt in context.page_counter
                            join cont in context.contract on cnt.ContractId equals cont.Id
                            join comp in context.company on cont.CompanyId equals comp.Id
                            select new { PageCounter = cnt, Contract = cont, Company = comp };
                var list = await query.ToListAsync();
                return list.Select(i => new PageCounterViewModel(i.PageCounter, i.Contract, i.Company));
            }
        }

        public static async Task AddUpdateCompany(CompanyViewModel companyVm)
        {
            using (var context = new PrintersPageComputationModel())
            {
                var company = await context.company.FirstOrDefaultAsync(i => i.Id == companyVm.Id);
                if (company == null)
                {
                    company = new company();
                    context.company.Add(company);
                }
                company.CompanyName = companyVm.CompanyName;
                await context.SaveChangesAsync();

                companyVm.Id = (int)company.Id;
            }
        }

        public static async Task AddUpdateContract(ContractViewModel contractVm)
        {
            using (var context = new PrintersPageComputationModel())
            {
                var contract = await context.contract.FirstOrDefaultAsync(i => i.Id == contractVm.Id);
                if (contract == null)
                {
                    contract = new contract();
                    context.contract.Add(contract);
                }
                contract.ContractNumber = contractVm.ContractNumber;
                contract.CompanyId = contractVm.Company.Id;
                contract.Description = contractVm.Description;
                contract.Department = contractVm.Department;
                contract.AssetModel = contractVm.AssetModel;
                contract.AssetType = contractVm.AssetType.ToString();
                contract.ExpirationDate = contractVm.ExpirationDate.ToUnixLong();
                contract.BasicPrice = contractVm.BasicPrice;
                contract.PageLimit_Black_A4 = contractVm.PageLimit_Black_A4;
                contract.PageLimit_Black_A3 = contractVm.PageLimit_Black_A3;
                contract.PageLimit_Colored_A4 = contractVm.PageLimit_Colored_A4;
                contract.PageLimit_Colored_A3 = contractVm.PageLimit_Colored_A3;
                contract.ExcessPrice_Black_A4 = contractVm.ExcessPrice_Black_A4;
                contract.ExcessPrice_Black_A3 = contractVm.ExcessPrice_Black_A3;
                contract.ExcessPrice_Colored_A4 = contractVm.ExcessPrice_Colored_A4;
                contract.ExcessPrice_Colored_A3 = contractVm.ExcessPrice_Colored_A3;
                await context.SaveChangesAsync();

                contractVm.Id = (int)contract.Id;
            }
        }

        public static async Task AddUpdatePageCounter(PageCounterViewModel counterVm)
        {
            using (var context = new PrintersPageComputationModel())
            {
                var counter = await context.page_counter.FirstOrDefaultAsync(i => i.Id == counterVm.Id);
                if (counter == null)
                {
                    counter = new page_counter();
                    context.page_counter.Add(counter);
                }
                counter.ContractId = counterVm.Contract.Id;
                counter.Date = counterVm.Date.ToUnixLong();
                counter.PageCount_Black_A4 = counterVm.PageCount_Black_A4;
                counter.PageCount_Black_A3 = counterVm.PageCount_Black_A3;
                counter.PageCount_Colored_A4 = counterVm.PageCount_Colored_A4;
                counter.PageCount_Colored_A3 = counterVm.PageCount_Colored_A3;
                counter.FileAttachment = ProcessFile(counter.FileAttachment, counterVm.FileAttachment, COUNTER_FILE_DIRECTORY);
                counter.IsReplaced = (int)counterVm.IsReplaced.ToLong();
                counter.UnitName = counterVm.UnitName;
                counter.UnitSerialNumber = counterVm.UnitSerialNumber;
                await context.SaveChangesAsync();

                counterVm.Id = (int)counter.Id;
                counterVm.FileAttachment = counter.FileAttachment;
            }
        }

        public static async Task DeletePageCounter(PageCounterViewModel counterVm)
        {
            using (var context = new PrintersPageComputationModel())
            {
                var counter = await context.page_counter.FirstOrDefaultAsync(i => i.Id == counterVm.Id);
                if (counter != null)
                {
                    context.page_counter.Remove(counter);
                    await context.SaveChangesAsync();
                    DeleteFile(counter.FileAttachment, COUNTER_FILE_DIRECTORY);
                }
            }
        }

        public static async Task<PageCounterViewModel> GetPreviousPeriodCounterAsync(int contractId, DateTime date)
        {
            using (var context = new PrintersPageComputationModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var dateUnix = date.ToUnixLong();
                var query = from count in context.page_counter
                            where count.ContractId == contractId && count.Date < dateUnix
                            orderby count.Date descending
                            select count;
                var result = await query.FirstOrDefaultAsync();
                return result == null ? null : new PageCounterViewModel(result, null, null);
            }
        }

        public static async Task<IEnumerable<ComputationGroupViewModel>> GetComputationListAsync()
        {
            using (var context = new PrintersPageComputationModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from computation in context.computation
                            join contract in context.contract on computation.ContractId equals contract.Id
                            join company in context.company on contract.CompanyId equals company.Id
                            join firstCounter in context.page_counter on computation.FirstPageCounterId equals firstCounter.Id
                            join lastCounter in context.page_counter on computation.LastPageCounterId equals lastCounter.Id
                            select new
                            {
                                computation,
                                contract,
                                company,
                                firstCounter,
                                lastCounter
                            };
                var list = await query.ToListAsync();
                return list.Select(i => new ComputationGroupViewModel(i.computation, i.contract, i.company, i.firstCounter, i.lastCounter));
            }
        }

        public static async Task<IEnumerable<PageCounterViewModel>> GetPageCountListFromPeriodAsync(DateTime fromPeriod, DateTime toPeriod, int contractId)
        {
            using (var context = new PrintersPageComputationModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                long fromPeriodUnix = fromPeriod.ToUnixLong();
                long toPeriodUnix = toPeriod.ToUnixLong();
                var query = from counter in context.page_counter
                            where counter.ContractId == contractId && counter.Date >= fromPeriodUnix && counter.Date <= toPeriodUnix
                            orderby counter.Date ascending
                            select counter;
                var list = await query.ToListAsync();
                return list.Select(i => new PageCounterViewModel(i, null, null));
            }
        }

        public static async Task SaveComputationReport(ComputationGroupViewModel computationGroup)
        {
            using (var context = new PrintersPageComputationModel())
            {
                var computation = await context.computation.FirstOrDefaultAsync(i => i.Id == computationGroup.Id);
                //var computation = await context.computation.FirstOrDefaultAsync(i => i.ContractId == computationGroup.Contract.Id && 
                //                                                            i.FirstPageCounterId == computationGroup.FirstCounter.Id && 
                //                                                            i.LastPageCounterId == computationGroup.LastCounter.Id);
                if (computation == null)
                {
                    computation = new computation();
                    context.computation.Add(computation);
                }
                computation.ContractId = computationGroup.Contract.Id;
                computation.FirstPageCounterId = computationGroup.FirstCounter.Id;
                computation.LastPageCounterId = computationGroup.LastCounter.Id;
                computation.ConsumptionMonth = computationGroup.ConsumptionMonth;
                computation.ConsumptionYear = computationGroup.ConsumptionYear;
                computation.CreatedDate = computationGroup.CreatedDate.ToUnixLong();
                computation.JustificationFile = ProcessFile(computation.JustificationFile, computationGroup.JustificationFile, JUSTIFICATION_FILE_DIRECTORY, string.Format("{0}_{1}_{2:MM-dd-yyyy} to {3:MM-dd-yyyy}", computationGroup.Contract.Company.CompanyName, computationGroup.ConsumptionDate.ToString("MMMM yyyy", CultureInfo.InvariantCulture), computationGroup.FirstCounter.Date, computationGroup.LastCounter.Date));
                await context.SaveChangesAsync();

                computationGroup.Id = (int)computation.Id;
                computationGroup.JustificationFile = computation.JustificationFile;
            }
        }

        public static async Task DeleteComputationReport(ComputationGroupViewModel computationGroup)
        {
            using (var context = new PrintersPageComputationModel())
            {
                var computation = await context.computation.FirstOrDefaultAsync(i => i.Id == computationGroup.Id);
                if (computation != null)
                {
                    context.computation.Remove(computation);
                    await context.SaveChangesAsync();
                    DeleteFile(computationGroup.JustificationFile, JUSTIFICATION_FILE_DIRECTORY);
                }
            }
        }
    }
}
