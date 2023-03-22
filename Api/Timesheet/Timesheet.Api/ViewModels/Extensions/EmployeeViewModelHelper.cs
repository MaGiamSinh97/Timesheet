using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Timesheet.Api.ViewModels.Extensions
{
    public static class EmployeeViewModelHelper
    {
        public static EmployeeViewModel ToViewModel(this Core.Employee model) =>
             new EmployeeViewModel()
             {
                 EmployeeId = model.Id,
                 EmployeeName = model.FullName,
                 AccNo = model.AccNo,
                 Du = model.Du,
                 KnoxId = model.KnoxId
             };


        public static IEnumerable<EmployeeViewModel> ToViewModels(this IEnumerable<Core.Employee> model)
            => model.Select(t => t.ToViewModel());

    }
}
