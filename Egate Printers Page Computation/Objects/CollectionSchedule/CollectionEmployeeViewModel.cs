using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionSchedule.Model;

namespace Egate_Printers_Page_Computation.Objects.CollectionSchedule
{
    public class CollectionEmployeeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        public CollectionEmployeeViewModel()
        { }

        public CollectionEmployeeViewModel(employee entity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.EmployeeName = entity.EmployeeName;
            this.Description = entity.Description;
            this.IsActive = entity.IsActive.ToBool();
        }

        public override bool Equals(object obj)
        {
            if (obj is CollectionEmployeeViewModel)
            {
                var o = obj as CollectionEmployeeViewModel;
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
