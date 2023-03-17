using System.Collections.Generic;
using System.Linq;

namespace Timesheet.Api.ViewModels.Extensions
{
    public static class ProjectViewModelHelper
    {
        public static ProjectViewModel ToViewModel(this Core.Project model) =>
            new ProjectViewModel
            {
                projectId = model.Id,
                projectName = model.Name,
                Du= model.Du,
                Description = model.Description,
            };
        public static IEnumerable<ProjectViewModel> ToViewModels(this IEnumerable<Core.Project> model) =>
             model.Select(t => t.ToViewModel());
    }
}
