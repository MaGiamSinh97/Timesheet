using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Infrastructure.Persistence;

namespace Timesheet.Api.Services
{
    public class EmployeeService : IGetAll<Core.Employee>, IGet<Core.Employee>
    {
        private readonly TimesheetContext context;

        public EmployeeService(TimesheetContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Core.Employee>> GetAllAsync(IEnumerable<int> ids)
        {
            return await this.context.Employees.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Core.Employee>> GetAllAsync()
        {
            return await this.context.Employees.Include("Timesheets").AsNoTracking().ToListAsync();
        }

        public async System.Threading.Tasks.Task Add(Timesheet.Core.Employee employee)
        {
            await this.context.AddAsync(employee);
            await this.context.SaveChangesAsync();
        }

        public async Task<Core.Employee> GetAsync(int id)
        {
            return await this.context.Employees.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        }  

        public bool CheckDuplicate(string Ldap)
        {
            return this.context.Employees.Any(t => t.Ldap == Ldap);
        }
        public async Task<Core.Employee> FindAsync(string ldap)
        {
            return await this.context.Employees.Where(t => t.Ldap == ldap).FirstOrDefaultAsync();
        }
    }
}
