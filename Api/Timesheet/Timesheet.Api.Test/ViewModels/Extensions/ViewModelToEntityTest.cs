using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timesheet.Api.ViewModels;
using Timesheet.Api.ViewModels.Extensions;
using Timesheet.Core;
using Timesheet.Infrastructure.Persistence;
using Xunit;

namespace Timesheet.Api.Test.ViewModels.Extensions
{ 
    public class ViewModelToEntityTest
    {
        [Fact]
        public void TimecardViewModel_ConvertToModel_CorrectDataConverted()
        {
            var expected = new Core.Timesheet() { Id = 1, Date = new DateTime(2020,6,25)};
            //var viewmodel = new TimesheetViewModel() { Id = 1, Date = new DateTime(2020, 6, 25) };
            //Assert.Equal(expected.Id,viewmodel.ToTimesheetEntity().Id);

        }
    }
}
