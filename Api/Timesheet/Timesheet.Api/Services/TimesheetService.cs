using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Core;
using Timesheet.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace Timesheet.Api.Services
{
    public class TimesheetService : IGetAll<Core.Timesheet>, IGet<Core.Timesheet>
    {
        private readonly TimesheetContext context;

        public TimesheetService(TimesheetContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Core.Timesheet>> GetAllAsync()
        {
            return await this.context.Timesheets.ToListAsync();
        }

        public async Task<IEnumerable<Core.Timesheet>> GetAllAsync(IEnumerable<int> ids)
        {
            return await this.context.Timesheets.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Core.Timesheet>> GetAllByDateRange(DateTime fromDate, DateTime toDate)
        {
            return await this.context.Timesheets.Where(t => t.Date >= fromDate && t.Date <= toDate).ToListAsync();
        }

        public async Task AddRange(List<Core.Timesheet> timesheet)
        {
            await this.context.Timesheets.AddRangeAsync(timesheet);
            await this.context.SaveChangesAsync();
        }
        public async Task Add(Core.Timesheet timesheet)
        {
            await this.context.Timesheets.AddAsync(timesheet);
            await this.context.SaveChangesAsync();
        }

        public async Task<Core.Timesheet> GetAsync(int id)
        {
            return await this.context.Timesheets.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        }
    }
}
