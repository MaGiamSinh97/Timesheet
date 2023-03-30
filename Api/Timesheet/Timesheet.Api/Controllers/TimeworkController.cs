using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2019.Drawing.Ink;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Timesheet.Api.Helpers;
using Timesheet.Api.Services;
using Timesheet.Api.ViewModels;
using Timesheet.Api.ViewModels.Extensions;
using Timesheet.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Timesheet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeworkController : ControllerBase
    {
        private readonly TimeworkService service;

        public TimeworkController(TimeworkService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // GET: api/<TimesheetController>

        [HttpGet] // the actions are the actual endpoints
        [HttpHead]
        public async Task<ActionResult<Timesheet.Core.Project[]>> Get()
        {
            try
            {
                var timeworks = await this.service.GetAllAsync();
                return Ok(timeworks);
            }
            catch (Exception)
            {
                //requestResult = this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        // GET api/<TimesheetController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Core.Project>> Get(int id)
        {
            var timeWork = await this.service.GetAsync(id);
            if (timeWork != null)
            {
                return Ok(timeWork);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Core.TimeWork[]>> Post(TimeworkViewModel model)
        {
            model.Id = 0;
            var timeWork = new TimeWork();
            var defaulDate = new DateTime(1899,12,31);
            timeWork.Type = model.Type;
            timeWork.TimeIn = defaulDate.Add(TimeSpan.Parse(model.TimeIn));
            timeWork.TimeOut = defaulDate.Add(TimeSpan.Parse(model.TimeOut));
            if(model.StartApply != "" && model.EndApply != "")
            {
                timeWork.StartApply = DateTime.ParseExact(model.StartApply.Substring(0,10), "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture).AddDays(1);
                timeWork.EndApply = DateTime.ParseExact(model.EndApply.Substring(0, 10), "yyyy-MM-dd",
                                      System.Globalization.CultureInfo.InvariantCulture).AddDays(1);
            }
            var result = await this.service.Add(timeWork);
            if (result > 0)
            {
                return Ok("Added Successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
            }
        }

        // DELETE api/<TimesheetController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Core.TimeWork[]>> Delete(int Id)
        {
            var result = await this.service.Delete(Id);
            if (result > 0)
            {
                return Ok("Deleted Successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
            }
        }

        [HttpOptions]
        public IActionResult GetProjectOptions()
        {
            Response.Headers.Add("Allow", string.Join(',', "GET", "OPTIONS", "POST", "DELETE"));

            // provide response body(not covered by http standard)
            return Ok();
        }
    }
}
