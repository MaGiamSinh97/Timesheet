using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Timesheet.Core
{
    public class Employee
    {
        public Employee()
        {

            this.Timesheets = new List<Timesheet>();

        }
        public int Id { get; set; }
        public string Ldap { get; set; }
        public string FullName { get; set; }
        public string Project { get; set; }
        public string Du { get; set; }

        public int AccNo { get; set; }
        public ICollection<Timesheet> Timesheets { get; set; }

    }
}
