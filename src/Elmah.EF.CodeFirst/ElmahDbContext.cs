namespace Elmah.EF.CodeFirst
{
    using Migrations;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Diagnostics;
    using System.Linq;

    public class ElmahDbContext : DbContext
    {
        public ElmahDbContext()
            : this("name=ElmahDbContext")
        {
        }

        public ElmahDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public virtual DbSet<ELMAH_Error> Error { get { return base.Set<ELMAH_Error>(); } }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ELMAH_Error>();
            entity.ToTable("Elmah_Error");
            entity.HasKey(p => p.ErrorId);
            entity.Property(p => p.ErrorId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            entity.Property(p => p.Sequence).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("Idx_ElmahLog_Clustered") { IsClustered = true }));
            entity.Property(p => p.Application).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ELMAH_Error_App_Time_Seq", 1) { IsClustered = false }));
            entity.Property(p => p.TimeUtc).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ELMAH_Error_App_Time_Seq", 2) { IsClustered = false }));
            entity.Property(p => p.Sequence).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ELMAH_Error_App_Time_Seq", 3) { IsClustered = false }));
        }
    }
}