using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timesheet.Api.Services;
using Timesheet.Core;
using Timesheet.Infrastructure.Persistence;
using Xunit;

namespace Timesheet.Api.Test.Services
{
    public class TimecardServiceTest
    {
        [Fact]
        public async System.Threading.Tasks.Task GetAll_ReturnsAllData()
        {
            var options = new DbContextOptionsBuilder<TimesheetContext>()
               .UseInMemoryDatabase(databaseName: "Timesheet1").Options;

            // Insert seed data into the database using one instance of the context
            var context = new TimesheetContext(options);

            context.Timesheets.Add(new Core.Timesheet() { Id = 1, Date = new DateTime(2020, 1,11)  });
            context.Timesheets.Add(new Core.Timesheet() { Id = 2, Date = new DateTime(2020, 1, 11) });
            context.Timesheets.Add(new Core.Timesheet() { Id = 3, Date = new DateTime(2020, 1, 11) });

            context.SaveChanges();

            var service = new TimesheetService(context);
            var all = await service.GetAllAsync();
            Assert.Equal(3, all.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllByDateRange_ReturnsCorrectNumberOfData()
        {
            //var options = new DbContextOptionsBuilder<TimesheetContext>()
            //   .UseInMemoryDatabase(databaseName: "Timesheet2").Options;

            //// Insert seed data into the database using one instance of the context
            //var context = new TimesheetContext(options);

            //context.Timesheets.Add(new Core.Timesheet() { Id = 1, Date = new DateTime(2020, 1, 11), TimeIn = 1, Timeout = 1 });
            //context.Timesheets.Add(new Core.Timesheet() { Id = 2, Date = new DateTime(2020, 1, 11), TimeIn = 2, Timeout = 1 });
            //context.Timesheets.Add(new Core.Timesheet() { Id = 3, Date = new DateTime(2019, 12, 31), TimeIn = 2, Timeout = 1 });
            //context.Timesheets.Add(new Core.Timesheet() { Id = 4, Date = new DateTime(2020, 1, 12), TimeIn = 2, Timeout = 1 });
            //context.Timesheets.Add(new Core.Timesheet() { Id = 6, Date = new DateTime(2020, 1, 13), TimeIn = 3, Timeout = 1 });
            //context.Timesheets.Add(new Core.Timesheet() { Id = 7, Date = new DateTime(2020, 1, 14), TimeIn = 4, Timeout = 1 });
            //context.Timesheets.Add(new Core.Timesheet() { Id = 8, Date = new DateTime(2020, 1, 15), TimeIn = 5, Timeout = 1 });

            //context.SaveChanges();

            //var service = new TimesheetService(context);
            //var all = await service.GetAllByDateRange(new DateTime(2020,1,11),new DateTime(2020,1,14));
            //all.ToList().ForEach(t => Console.WriteLine(t.Id));
            //Assert.Equal(5, all.Count());
        }
    }
}
