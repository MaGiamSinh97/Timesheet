using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Timesheet.Infrastructure.Persistence;

namespace Timesheet.Api
{
    public static class StartupHelper
    {
        public static void SeedDataTimesheetContext(this IApplicationBuilder application)
        {
            using (var serviceScope = application.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<TimesheetContext>();

                context.Database.EnsureCreated();

                var databaseSeeder = new Infrastructure.Helpers.DataInitializer(context);
                //if (!context.Timecards.Any())
                //{
                //    databaseSeeder.SeedTasks();
                //}
                //if (!context.Timecards.Any())
                //{
                //    databaseSeeder.SeedTimesheets();
                //}
            }
        }
    }
}
