using Timesheet.Core;

namespace Timesheet.Api.ViewModels
{
    public class TimeworkViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int Type { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string StartApply { get; set; }
        public string EndApply { get; set; }
    }
}
