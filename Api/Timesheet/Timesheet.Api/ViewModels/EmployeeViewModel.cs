using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheet.Api.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public int AccNo { get; set; }
        public string EmployeeName { get; set; }
        public string KnoxId { get; set; }
        public string Du { get; set; }
        public ICollection<TimesheetViewModel> Timesheets { get; set; }
    }
}
