using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintersPageComputation.Model;

namespace Egate_Printers_Page_Computation.Objects
{
    public class CompanyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; } //database reference
        public string CompanyName { get; set; }
        public bool IsActive { get; set; } = true;

        public int ContractCount { get; set; }

        public CompanyViewModel()
        { }

        public CompanyViewModel(company entity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.CompanyName = entity.CompanyName;
            this.IsActive = entity.IsActive.ToBool();
        }

        public override bool Equals(object obj)
        {
            if (obj is CompanyViewModel)
            {
                var o = obj as CompanyViewModel;
                return this.Id == o.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
