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
        public bool CheckDuplicate(string name)
        {
            return this.context.Projects.Any(t => t.Name == name);
        }
        public async Task<Core.Project> FindAsync(string name)
        {
            return await this.context.Projects.Where(t => t.Name == name).FirstOrDefaultAsync();
        }
    }
}