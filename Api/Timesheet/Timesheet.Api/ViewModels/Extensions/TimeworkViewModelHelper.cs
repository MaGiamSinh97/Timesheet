using System.Collections.Generic;
using System.Linq;

namespace Timesheet.Api.ViewModels.Extensions
{
    public static class TimeworkViewModelHelper
    {
        public static TimeworkViewModel ToViewModel(this Core.TimeWork model) =>
            new TimeworkViewModel
            {
               Id = model.Id,
               TimeIn = model.TimeIn.ToString("HH:mm"),
               TimeOut = model.TimeOut.ToString("HH:mm"),
               StartApply = model.StartApply.ToString("yyyy/MM/dd"),
               EndApply = model.EndApply.ToString("yyyy/MM/dd"),
               Type = model.Type,
               EmployeeId = model.EmployeeId,
            };
        public static IEnumerable<TimeworkViewModel> ToViewModels(this IEnumerable<Core.TimeWork> model) =>
             model.Select(t => t.ToViewModel());
    }
}
