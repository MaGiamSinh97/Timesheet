using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Timesheet.Api.ViewModels.Extensions
{
    public static class EmployeeViewModelHelper
    {
        public static double AbsentTime(DateTime timeIn, DateTime timeOut)
        {
            double absentTime = 0;
            var timemorning = DateTime.Parse("1899/12/31 8:00:00.000");
            var timeafternoon = DateTime.Parse("1899/12/31 17:00:00.000");
            if (timeIn.TimeOfDay > timemorning.TimeOfDay)
            {
                absentTime = timeIn.Subtract(timemorning).TotalMinutes;
            }
            if (timeOut.TimeOfDay < timeafternoon.TimeOfDay)
            {
                absentTime += timeafternoon.Subtract(timeOut).TotalMinutes;
            }
            return absentTime;
        }
        public static EmployeeViewModel ToViewModel(this Core.Employee model) =>
             new EmployeeViewModel()
             {
                 Name = model.FullName,
                 AccNo = model.AccNo,
                 Du = model.Du,
                 Ldap = model.Ldap,
                 Project = model.Project,
                 Timesheets = model.Timesheets.Select(t => t.ToViewTimesheetModel()).ToList()
             };


        public static IEnumerable<EmployeeViewModel> ToViewModels(this IEnumerable<Core.Employee> model)
            => model.Select(t => t.ToViewModel());

        public static TimesheetViewModel ToViewTimesheetModel(this Core.Timesheet model) =>
          new TimesheetViewModel
          {
              Ldap = model.Employee.Ldap,
              Name = model.Employee.FullName,
              Id = model.Id,
              Date = model.Date,
              TimeIn = model.TimeIn,
              TimeOut = model.Timeout,
              AbsentTime = (int)AbsentTime(model.TimeIn, model.Timeout),
          };
    }
}
