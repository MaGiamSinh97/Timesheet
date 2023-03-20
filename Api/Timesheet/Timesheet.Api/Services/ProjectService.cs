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
    public class ProjectService : IGetAll<Core.Project>, IGet<Core.Project>
    {
        private readonly TimesheetContext context;

        public ProjectService(TimesheetContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Core.Project>> GetAllAsync(IEnumerable<int> ids)
        {
            return await this.context.Projects.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Core.Project>> GetAllAsync()
        {
            return await this.context.Projects.AsNoTracking().ToListAsync();
        }
        public async Task<Core.Project> GetAsync(int id)
        {
            return await this.context.Projects.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        }

        public async System.Threading.Tasks.Task<int> Add(Timesheet.Core.Project project)
        {
            await this.context.AddAsync(project);
            return await this.context.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task<int> Update(Timesheet.Core.Project project)
        {
            this.context.Entry(project).State = EntityState.Modified;
            return await this.context.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task<int> Delete(int Id)
        {
            var project = this.context.Projects.Find(Id);
            if (project != null)
            {
                this.context.Projects.Entry(project).State = EntityState.Deleted;
                return await this.context.SaveChangesAsync();
            }
            return 0;
        }
        public async System.Threading.Tasks.Task<int> RemoveMember(int proId, int empId)
        {
            var projectEmp = this.context.ProjectEmployees.First(row => row.ProjectId == proId && row.EmployeeId == empId);
            if (projectEmp != null)
            {
                this.context.ProjectEmployees.Entry(projectEmp).State = EntityState.Deleted;
                return await this.context.SaveChangesAsync();
            }
            return 0;
        }
        public async System.Threading.Tasks.Task<int> AddMember(ProjectEmployee projectEmployee)
        {
            this.context.ProjectEmployees.Add(projectEmployee);
            return await this.context.SaveChangesAsync();

        }
        public bool CheckDuplicate(string name)
        {
            return this.context.Projects.Any(t => t.Name == name);
        }
        public async Task<Core.Project> FindAsync(string name)
        {
            return await this.context.Projects.Where(t => t.Name == name).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Core.Employee>> GetMember(int projectId)
        {
            var result = await this.context.ProjectEmployees.Include(x => x.Employee).Where(emp => emp.ProjectId == projectId).Select(emp => emp.Employee).ToListAsync();
            return result;
        }

    }
}