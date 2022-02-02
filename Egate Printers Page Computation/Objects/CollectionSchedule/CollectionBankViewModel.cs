using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionSchedule.Model;

namespace Egate_Printers_Page_Computation.Objects.CollectionSchedule
{
    public class CollectionBankViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public string BankName { get; set; }

        public CollectionBankViewModel()
        { }

        public CollectionBankViewModel(bank entity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.BankName = entity.BankName;
        }

        public override bool Equals(object obj)
        {
            if (obj is CollectionBankViewModel)
            {
                var o = obj as CollectionBankViewModel;
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
