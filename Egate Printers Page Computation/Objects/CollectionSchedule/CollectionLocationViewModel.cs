using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionSchedule.Model;

namespace Egate_Printers_Page_Computation.Objects.CollectionSchedule
{
    public class CollectionLocationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public string LocationName { get; set; }

        public CollectionLocationViewModel()
        { }

        public CollectionLocationViewModel(location entity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.LocationName = entity.LocationName;
        }

        public override bool Equals(object obj)
        {
            if (obj is CollectionLocationViewModel)
            {
                var o = obj as CollectionLocationViewModel;
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
