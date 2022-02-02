using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace PrintersPageComputation.Model
{
    public partial class PrintersPageComputationModel : DbContext
    {
        public PrintersPageComputationModel()
            : base("name=PrintersPageComputationModel")
        {
        }

        public virtual DbSet<company> company { get; set; }
        public virtual DbSet<contract> contract { get; set; }
        public virtual DbSet<page_counter> page_counter { get; set; }
        public virtual DbSet<computation> computation { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(10, 2));

            modelBuilder.Entity<company>()
                .HasMany(e => e.contracts)
                .WithRequired(e => e.company)
                .HasForeignKey(e => e.CompanyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<contract>()
                .HasMany(e => e.page_counters)
                .WithRequired(e => e.contract)
                .HasForeignKey(e => e.ContractId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<computation>()
                .HasRequired(e => e.contract)
                .WithMany(e => e.computations)
                .HasForeignKey(e => e.ContractId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<computation>()
                .HasRequired(e => e.first_page_counter)
                .WithMany(e => e.first_page_counter_computations)
                .HasForeignKey(e => e.FirstPageCounterId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<computation>()
                .HasRequired(e => e.last_page_counter)
                .WithMany(e => e.last_page_counter_computations)
                .HasForeignKey(e => e.LastPageCounterId)
                .WillCascadeOnDelete(true);
        }

        public void Initialize()
        {
            Database.SetInitializer<PrintersPageComputationModel>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Database.Initialize(false);
            this.contract.Load();
            Configuration.AutoDetectChangesEnabled = true;
        }
    }
}
