using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Core;
using Timesheet.Infrastructure.Persistence;

namespace Timesheet.Api.Services
{
    public class ProjectEmployeeService : IGetAll<Core.ProjectEmployee>
    {
        private readonly TimesheetContext context;

        public ProjectEmployeeService(TimesheetContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Core.ProjectEmployee>> GetAllAsync()
        {
            return await this.context.ProjectEmployees.AsNoTracking().ToListAsync();
        }

        public async System.Threading.Tasks.Task Add(Timesheet.Core.ProjectEmployee projectEmployee)
        {
            await this.context.AddAsync(projectEmployee);
            await this.context.SaveChangesAsync();
        }

        public Task<IEnumerable<ProjectEmployee>> GetAllAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
        public bool CheckDuplicate(int pid, int eid)
        {
            return this.context.ProjectEmployees.Any(t => t.ProjectId == pid && t.EmployeeId == eid);
        }
    }
}