using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheet.Api.ViewModels
{
    public class EmployeeViewModel
    {
        public int AccNo { get; set; }
        public string Name { get; set; }
        public string Ldap { get; set; }
        public string Project { get; set; }
        public string Du { get; set; }
        public ICollection<TimesheetViewModel> Timesheets { get; set; }
    }
}
