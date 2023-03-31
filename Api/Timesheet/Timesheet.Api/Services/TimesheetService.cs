using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Core;
using Timesheet.Infrastructure.Persistence;

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
            return await this.context.Timesheets.Include(x => x.Employee).ThenInclude(x => x.ProjectEmployees).ThenInclude(x => x.Project).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Core.Timesheet>> GetAllAsync(IEnumerable<int> ids)
        {
            return await this.context.Timesheets.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Core.Timesheet>> GetAllByDateRange(DateTime fromDate, DateTime toDate)
        {
            return await this.context.Timesheets.Include(x => x.Employee).ThenInclude(x => x.ProjectEmployees).ThenInclude(x => x.Project).AsNoTracking().Where(t => t.Date >= fromDate && t.Date <= toDate).ToListAsync();
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
            return await this.context.Timesheets.Include(x=>x.Employee).AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        }
        public double AbsentTime(Core.Timesheet model)
        {
            double absentTime = 0;
            var timeworks = this.context.TimeWorks.Where(x => x.EmployeeId == model.Employee.Id).ToList();
            if (!timeworks.Any())
            {
                var timemorning = DateTime.Parse("1899/12/31 8:00:00.000");
                var timeafternoon = DateTime.Parse("1899/12/31 17:00:00.000");
                if (model.TimeIn.TimeOfDay > timemorning.TimeOfDay)
                {
                    absentTime = model.TimeIn.Subtract(timemorning).TotalMinutes;
                }
                if (model.Timeout.TimeOfDay < timeafternoon.TimeOfDay)
                {
                    absentTime += timeafternoon.Subtract(model.Timeout).TotalMinutes;
                }
            }
            else
            {
                var fixedtime = timeworks.Where(x => x.Type == 1).ToList();
                if (fixedtime.Any())
                {
                    foreach (var item in fixedtime)
                    {
                        if (item.StartApply <= model.Date && item.EndApply >= model.Date)
                        {
                            var timemorning = item.TimeIn;
                            var timeafternoon = item.TimeOut;
                            if (model.TimeIn.TimeOfDay > timemorning.TimeOfDay)
                            {
                                absentTime = model.TimeIn.Subtract(timemorning).TotalMinutes;
                            }
                            if (model.Timeout.TimeOfDay < timeafternoon.TimeOfDay)
                            {
                                absentTime += timeafternoon.Subtract(model.Timeout).TotalMinutes;
                            }
                        }
                    }
                }
                var flexitime = timeworks.Where(x => x.Type == 2).ToList();
                if (flexitime.Any())
                {
                    foreach(var item in flexitime)
                    {
                        var timemorning = item.TimeIn;
                        var timeafternoon = item.TimeOut; 
                        var range = item.TimeOut.Subtract(item.TimeOut).TotalMinutes;
                        if(model.TimeIn.TimeOfDay > timemorning.TimeOfDay && model.TimeIn.TimeOfDay < timeafternoon.TimeOfDay)
                        {
                            var timework = model.Timeout.Subtract(model.TimeIn).TotalMinutes;
                            if(timework < 9 * 60)
                            {
                                absentTime = (9 * 60) - timework;
                            }
                            
                        }
                        if(model.TimeIn.TimeOfDay > timeafternoon.TimeOfDay)
                        {
                            absentTime = model.TimeIn.Subtract(timeafternoon).TotalMinutes;
                        }
                    }
                }
            }

            return absentTime;
        }
    }
}
