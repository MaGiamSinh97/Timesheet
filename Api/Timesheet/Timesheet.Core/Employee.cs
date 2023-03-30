using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timesheet.Core
{
    public class Employee
    {
        public Employee()
        {
            this.Timesheets = new List<Timesheet>();
            this.Timeworks = new List<TimeWork>();
            this.ProjectEmployees = new List<ProjectEmployee>();
        }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string KnoxId { get; set; }
        public string FullName { get; set; }
        public string Du { get; set; }
        public int AccNo { get; set; }
        public string EncPass { get; set; }
        public byte[] StoredSalt { get; set; }
        public int Role { get; set; }
        public ICollection<Timesheet> Timesheets { get; set; }
        public ICollection<TimeWork> Timeworks { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
    }
}
