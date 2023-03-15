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
            modelBuilder.Entity<Project>().HasKey(tc => tc.Id);
            modelBuilder.Entity<ProjectEmployee>().HasKey(sc => new { sc.ProjectId, sc.EmployeeId });

            // Timecard and Task Relationship
            modelBuilder.Entity<Timesheet.Core.Timesheet>().HasOne(t => t.Employee)
                .WithMany(t => t.Timesheets);
            modelBuilder.Entity<ProjectEmployee>().HasOne(p => p.Project)
                .WithMany(p => p.ProjectEmployees).HasForeignKey(p => p.ProjectId); ;
            modelBuilder.Entity<ProjectEmployee>().HasOne(p => p.Employee)
                .WithMany(p => p.ProjectEmployees).HasForeignKey(p=>p.EmployeeId);
        }
    }
}
