using System.Collections.Generic;

namespace Timesheet.Core
{
    public class Employee
    {
        public Employee()
        {
            this.Timesheets = new List<Timesheet>();
            this.ProjectEmployees = new List<ProjectEmployee>();
        }
        public int Id { get; set; }
        public string KnoxId { get; set; }
        public string FullName { get; set; }
        public string Du { get; set; }
        public int AccNo { get; set; }
        public string EncPass { get; set; }
        public byte[] StoredSalt { get; set; }
        public int Role { get; set; }
        public ICollection<Timesheet> Timesheets { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
    }
}
