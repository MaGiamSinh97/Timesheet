using System;

namespace Timesheet.Api.ViewModels
{
    public class TimesheetViewModel
    {
        public int Id { get; set; }
        public string KnoxId { get; set; }
        public string Name { get; set; }
        public string Project { get; set; }
        public string Date { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public int AbsentTime { get; set; }
    }
}
