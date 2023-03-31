using System;
using System.Collections.Generic;
using System.Linq;
using Timesheet.Api.Services;

namespace Timesheet.Api.ViewModels.Extensions
{
    public static class TimesheetViewModelHelper
    {
        public static double AbsentTime(Core.Timesheet model)
        {
            double absentTime = 0;
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
                AbsentTime = (int)AbsentTime(model)            
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
                AbsentTime = (int)AbsentTime(model),
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
