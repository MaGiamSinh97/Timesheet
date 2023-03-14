using System.Collections.Generic;
using System.Linq;

namespace Timesheet.Api.ViewModels.Extensions
{
    public static class TimesheetViewModelHelper
    {
        public static TimesheetViewModel ToViewModel(this Core.Timesheet model) =>
            new TimesheetViewModel
            {
                Id = model.Id,
                Date = model.Date,
            };
       
        public static TimesheetViewModel ToCreateViewModel(this Core.Timesheet model) =>
        
            new TimesheetViewModel
            {
                Date = model.Date,
            };
        
        public static IEnumerable<TimesheetViewModel> ToViewModels(this IEnumerable<Core.Timesheet> model) =>
             model.Select(t => t.ToViewModel());

        public static IEnumerable<TimesheetViewModel> ToCreateViewModels(this IEnumerable<Core.Timesheet> model) =>
             model.Select(t => t.ToCreateViewModel());   
    }
}
