using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintersPageComputation.Model;

namespace Egate_Printers_Page_Computation.Objects
{
    public class ContractViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public CompanyViewModel Company { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string AssetModel { get; set; }
        public AssetType AssetType { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public decimal? BasicPrice { get; set; }
        public int? PageLimit_Black_A4 { get; set; }
        public int? PageLimit_Black_A3 { get; set; }
        public int? PageLimit_Colored_A4 { get; set; }
        public int? PageLimit_Colored_A3 { get; set; }
        public decimal? ExcessPrice_Black_A4 { get; set; }
        public decimal? ExcessPrice_Black_A3 { get; set; }
        public decimal? ExcessPrice_Colored_A4 { get; set; }
        public decimal? ExcessPrice_Colored_A3 { get; set; }

        public ContractViewModel()
        { }

        public ContractViewModel(contract entity, company companyEntity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.ContractNumber = entity.ContractNumber;
            this.Company = new CompanyViewModel(companyEntity);
            this.Description = entity.Description;
            this.Department = entity.Department;
            this.AssetModel = entity.AssetModel;
            this.AssetType = entity.AssetType.ToEnum<AssetType>();
            this.ExpirationDate = entity.ExpirationDate.ToUnixDate();
            this.BasicPrice = entity.BasicPrice;
            this.PageLimit_Black_A4 = entity.PageLimit_Black_A4;
            this.PageLimit_Black_A3 = entity.PageLimit_Black_A3;
            this.PageLimit_Colored_A4 = entity.PageLimit_Colored_A4;
            this.PageLimit_Colored_A3 = entity.PageLimit_Colored_A3;
            this.ExcessPrice_Black_A4 = entity.ExcessPrice_Black_A4;
            this.ExcessPrice_Black_A3 = entity.ExcessPrice_Black_A3;
            this.ExcessPrice_Colored_A4 = entity.ExcessPrice_Colored_A4;
            this.ExcessPrice_Colored_A3 = entity.ExcessPrice_Colored_A3;
        }
    }
}
