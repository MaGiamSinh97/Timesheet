using System;
using System.Collections.Generic;
using System.Text;

namespace Timesheet.Core
{
    public class Timesheet
    {
        public int Id;
        public Employee Employee;
        public DateTime Date { get; set; }

        public DateTime TimeIn { get; set; }

        public DateTime Timeout { get; set; }
    }
}
