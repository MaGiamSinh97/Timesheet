using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Core
{
    public class TimeWork
    {
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public int Type { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public DateTime StartApply { get; set; }
        public DateTime EndApply { get; set; }
    }
}
