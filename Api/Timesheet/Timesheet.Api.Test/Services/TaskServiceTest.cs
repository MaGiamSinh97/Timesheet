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
    public class TaskServiceTest
    {
        [Fact]
        public async System.Threading.Tasks.Task GetAll_ReturnsAllData()
        {
            var options = new DbContextOptionsBuilder<TimesheetContext>()
                .UseInMemoryDatabase(databaseName: "Timesheet").Options;

            // Insert seed data into the database using one instance of the context
            var context = new TimesheetContext(options);

            context.Employees.Add(new Employee() { FullName = "Test1" });
            context.Employees.Add(new Employee() { FullName = "Test2" });
            context.Employees.Add(new Employee() { FullName = "Test3" });
            context.SaveChanges();

            var service = new EmployeeService(context);
            var all = await service.GetAllAsync();
            Assert.Equal(3, all.Count());
        }
    }
}
