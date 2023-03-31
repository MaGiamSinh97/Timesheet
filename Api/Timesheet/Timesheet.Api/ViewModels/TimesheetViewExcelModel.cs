using System;

namespace Timesheet.Api.ViewModels
{
    public class TimesheetViewExcelModel
    {
        public int Id { get; set; }
        public string KnoxId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Du { get; set; }
        public string Project { get; set; }
        public int ProjectId { get; set; }
        public DateTime Date { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public double AbsentTime { get; set; }
        public double TotalAbsentTime { get; set; }
    }
}
