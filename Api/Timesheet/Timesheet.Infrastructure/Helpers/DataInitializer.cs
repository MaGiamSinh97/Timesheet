using System;
using System.Collections.Generic;
using Timesheet.Core;
using Timesheet.Infrastructure.Persistence;

namespace Timesheet.Infrastructure.Helpers
{
    public class DataInitializer
    {
        private readonly TimesheetContext context;

        public DataInitializer(TimesheetContext context)
        {
            this.context = context;
        }

        public void SeedTasks()
        {
        }

        public void SeedTimesheets()
        {
        }
    }
}
