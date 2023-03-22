using System;
using System.Collections.Generic;
using System.Linq;

namespace Timesheet.Api.ViewModels.Extensions
{
    public static class TimesheetViewModelHelper
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
        public static TimesheetViewModel ToViewModel(this Core.Timesheet model) =>
            new TimesheetViewModel
            {
                Id = model.Id,
                KnoxId = model.Employee.KnoxId,
                Name = model.Employee.FullName,
                Project = model.Employee.ProjectEmployees.Where(x=>x.EmployeeId == model.Employee.Id).FirstOrDefault() != null? model.Employee.ProjectEmployees.Where(x => x.EmployeeId == model.Employee.Id).FirstOrDefault().Project.Name:"",
                Date = model.Date.ToShortDateString(),
                TimeIn = model.TimeIn.ToString("HH:mm"),
                TimeOut = model.Timeout.ToString("HH:mm"),
                AbsentTime = (int)AbsentTime(model.TimeIn,model.Timeout)
            };
       
        public static TimesheetViewModel ToCreateViewModel(this Core.Timesheet model) =>
        
            new TimesheetViewModel
            {
                Date = model.Date.ToShortDateString(),
            };
        
        public static IEnumerable<TimesheetViewModel> ToViewModels(this IEnumerable<Core.Timesheet> model) =>
             model.Select(t => t.ToViewModel());

        public static IEnumerable<TimesheetViewModel> ToCreateViewModels(this IEnumerable<Core.Timesheet> model) =>
             model.Select(t => t.ToCreateViewModel());   
    }
}
