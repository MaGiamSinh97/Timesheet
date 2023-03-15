using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Core
{
    public class Project
    {
        public Project() 
        {
            this.ProjectEmployees = new List<ProjectEmployee>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Du { get; set; }
        public string Description { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
    }
}
