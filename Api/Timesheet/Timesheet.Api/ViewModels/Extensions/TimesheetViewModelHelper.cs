using System;
using System.Collections.Generic;
using System.Linq;
using Timesheet.Api.Services;

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

        public static int TotalAbsentTime(List<Core.Timesheet> timesheets)
        {
            int total = 0;

            return total;
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
       
        public static TimesheetViewExcelModel ToExcelViewModel(this Core.Timesheet model) =>
        
            new TimesheetViewExcelModel
            {
                Id = model.Id,
                KnoxId = model.Employee.KnoxId,
                EmployeeId = model.Employee.Id,
                Name = model.Employee.FullName,
                Date = model.Date,
                TimeIn = model.TimeIn.ToString("HH:mm"),
                TimeOut = model.Timeout.ToString("HH:mm"),
                AbsentTime = (int)AbsentTime(model.TimeIn, model.Timeout),
                Du = model.Employee.Du,
                ProjectId =  model.Employee.ProjectEmployees.Where(x => x.EmployeeId == model.Employee.Id).FirstOrDefault() != null ? model.Employee.ProjectEmployees.Where(x => x.EmployeeId == model.Employee.Id).FirstOrDefault().Project.Id : 0,
                Project = model.Employee.ProjectEmployees.Where(x => x.EmployeeId == model.Employee.Id).FirstOrDefault() != null ? model.Employee.ProjectEmployees.Where(x => x.EmployeeId == model.Employee.Id).FirstOrDefault().Project.Name : "",
            };
        
        public static IEnumerable<TimesheetViewModel> ToViewModels(this IEnumerable<Core.Timesheet> model) =>
             model.Select(t => t.ToViewModel());

        public static IEnumerable<TimesheetViewExcelModel> ToExcelViewModels(this IEnumerable<Core.Timesheet> model) =>
             model.Select(t => t.ToExcelViewModel());   
    }
}
