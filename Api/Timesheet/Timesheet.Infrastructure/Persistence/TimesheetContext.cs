using Microsoft.EntityFrameworkCore;
using Timesheet.Core;

namespace Timesheet.Infrastructure.Persistence
{
    public class TimesheetContext : DbContext
    {
        public DbSet<Timesheet.Core.Timesheet> Timesheets { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public TimesheetContext(DbContextOptions<TimesheetContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Timesheet.Core.Timesheet>().HasKey(tc => tc.Id);
            modelBuilder.Entity<Employee>().HasKey(tc => tc.Id);

            // Timecard and Task Relationship
            modelBuilder.Entity<Timesheet.Core.Timesheet>().HasOne(t => t.Employee)
                .WithMany(t => t.Timesheets);
        }
    }
}
