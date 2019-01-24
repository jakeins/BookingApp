using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookingApp.Models;
using System.Linq;

namespace BookingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options)
        : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<Rule> Rules { get; set; }
        public virtual DbSet<TreeGroup> TreeGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //necessary call of the base method
            base.OnModelCreating(modelBuilder);

            //setting default date autofill for all modification dates
            foreach (var entity in modelBuilder.Model.GetEntityTypes()
                .Where(et => et.FindProperty("CreatedDate") != null && et.FindProperty("UpdatedDate") != null)
                .Select(et => modelBuilder.Entity(et.ClrType)))
            {
                entity.Property("CreatedDate").HasDefaultValueSql("getdate()");
                entity.Property("UpdatedDate").HasDefaultValueSql("getdate()");
            }

            //setting default values
            var ruleEntity = modelBuilder.Entity<Rule>();
            ruleEntity.Property("MinTime").HasDefaultValue(1);
            ruleEntity.Property("MaxTime").HasDefaultValue(1440);
            ruleEntity.Property("StepTime").HasDefaultValue(1);
            ruleEntity.Property("ServiceTime").HasDefaultValue(0);
            ruleEntity.Property("ReuseTimeout").HasDefaultValue(0);
            ruleEntity.Property("PreOrderTimeLimit").HasDefaultValue(1440);

            var userEntity = modelBuilder.Entity<ApplicationUser>();
            userEntity.Property("IsApproved").HasDefaultValue(false);
            userEntity.Property("IsActive").HasDefaultValue(true);

            //resetting the default Delete behavior from Cascade to Restricted (i.e. No Action)
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
