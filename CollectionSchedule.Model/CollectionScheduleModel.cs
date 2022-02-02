using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace CollectionSchedule.Model
{
    public partial class CollectionScheduleModel : DbContext
    {
        public CollectionScheduleModel()
            : base("name=CollectionScheduleModel")
        {
        }

        public virtual DbSet<collection> collection { get; set; }
        public virtual DbSet<invoice> invoice { get; set; }
        public virtual DbSet<company> company { get; set; }
        public virtual DbSet<employee> employee { get; set; }
        public virtual DbSet<location> location { get; set; }
        public virtual DbSet<bank> bank { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(10, 2));

            modelBuilder.Entity<company>()
                .HasMany(e => e.collections)
                .WithRequired(e => e.company)
                .HasForeignKey(e => e.CompanyId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<company>()
                .HasMany(e => e.invoices)
                .WithRequired(e => e.company)
                .HasForeignKey(e => e.CompanyId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<location>()
                .HasMany(e => e.companies)
                .WithOptional(e => e.location)
                .HasForeignKey(e => e.LocationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<bank>()
                .HasMany(e => e.collections)
                .WithOptional(e => e.bank)
                .HasForeignKey(e => e.BankId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<employee>()
                .HasMany(e => e.collections_collected)
                .WithOptional(e => e.employee_collected_by)
                .HasForeignKey(e => e.CollectedByEmployeeId);

            modelBuilder.Entity<employee>()
                .HasMany(e => e.collections_updated)
                .WithOptional(e => e.employee_updated_by)
                .HasForeignKey(e => e.UpdatedByEmployeeId)
                .WillCascadeOnDelete(false);
        }

        public void Initialize()
        {
            Database.SetInitializer<CollectionScheduleModel>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Database.Initialize(false);
            this.collection.Load();
            Configuration.AutoDetectChangesEnabled = true;
        }
    }
}
