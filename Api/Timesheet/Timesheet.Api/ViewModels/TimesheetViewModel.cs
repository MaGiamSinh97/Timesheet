using System;

namespace Timesheet.Api.ViewModels
{
    public class TimesheetViewModel
    {
        public int Id { get; set; }
        public string Ldap { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public int AbsentTime { get; set; }
    }
}
