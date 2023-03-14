using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Core;

namespace Timesheet.Api.ViewModels.Extensions
{
    public static class ViewModelToEntity
    {
        public static Core.Timesheet ToTimesheetEntity(this TimesheetViewModel viewModel) =>
             new Core.Timesheet() { Id = viewModel.Id, Date = viewModel.Date };
        
        public static Core.Employee ToEmployeeEntity(this EmployeeViewModel viewModel) =>
             new Core.Employee() { FullName = viewModel.Name };

    }
}
