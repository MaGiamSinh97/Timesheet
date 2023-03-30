using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Infrastructure.Persistence;

namespace Timesheet.Api.Services
{
    public class TimeworkService : IGetAll<Core.TimeWork>, IGet<Core.TimeWork>
    {
        private readonly TimesheetContext context;

        public TimeworkService(TimesheetContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Core.TimeWork>> GetAllAsync()
        {
            return await this.context.TimeWorks.Include(x=>x.Employee).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Core.TimeWork>> GetAllAsync(IEnumerable<int> ids)
        {
            return await this.context.TimeWorks.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        public async Task AddRange(List<Core.TimeWork> timeWorks)
        {
            await this.context.TimeWorks.AddRangeAsync(timeWorks);
            await this.context.SaveChangesAsync();
        }
        public async Task<int> Add(Core.TimeWork timeWork)
        {
            try
            {
                await this.context.TimeWorks.AddAsync(timeWork);
                return await this.context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Core.TimeWork> GetAsync(int id)
        {
            return await this.context.TimeWorks.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        }
        public async Task<Core.TimeWork> GetTimeWorkbyEmployee(int employeeId)
        {
            return await this.context.TimeWorks.AsNoTracking().SingleOrDefaultAsync(t => t.Employee.Id == employeeId);
        }
        public async System.Threading.Tasks.Task<int> Delete(int Id)
        {
            var timeWork = this.context.TimeWorks.Find(Id);
            if (timeWork != null)
            {
                this.context.TimeWorks.Entry(timeWork).State = EntityState.Deleted;
                return await this.context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
