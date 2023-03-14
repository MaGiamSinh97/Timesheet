using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Timesheet.Api.Helpers;
using Timesheet.Api.Services;
using Timesheet.Api.ViewModels;
using Timesheet.Api.ViewModels.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Timesheet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly TimesheetService service;

        public TimesheetController(TimesheetService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // GET: api/<TimesheetController>

        [HttpGet] // the actions are the actual endpoints
        [HttpHead]
        public async Task<ActionResult<Timesheet.Core.Timesheet[]>> Get()
        {
            try
            {
                var timesheets = await this.service.GetAllAsync();
                return Ok(timesheets.ToViewModels());
            }
            catch (Exception)
            {
                //requestResult = this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        // GET api/<TimesheetController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Core.Timesheet>> Get(int id)
        {
            var timecard = await this.service.GetAsync(id);
            if (await this.service.GetAsync(id) != null)
            {
                return Ok(timecard);
            }

            return NotFound();
        }

        [HttpGet("({ids})", Name = nameof(TimesheetController.GetTimesheets))]
        public async Task<ActionResult<Core.Timesheet>> GetTimesheets([FromRoute]
        [ModelBinder(BinderType = typeof(CommaSeparatedModelBinder))] IEnumerable<int> ids)
        {
            if(ids == null)
            {
                return BadRequest();
            }

            var timecards = await this.service.GetAllAsync(ids);

            if(ids.Count() != timecards.Count())
            {
                // We return all, of if any is missing, we return not found
                return NotFound();
            }

            return Ok(timecards);
        }

        // POST api/<TimesheetController>
        //[HttpPost]
        //public async Task<ActionResult<Core.Timesheet>> Post([FromBody] IEnumerable<TimesheetViewModel> timesheets)
        //{
        //    //var timecardModels = timesheets.ToList();
        //    ////var url = this.linkGenerator.GetPathByAction(HttpContext, "Get", values: new { timecards = timecardModels });
        //    ////await this.service.Add(timecardModels);
        //    ////return Created(url, timecardModels);
        //    //await this.service.Add(timecardModels);
        //    //var idsAsString = string.Join(",", timecardModels.ToViewModels().Select(t => t.Id));
        //    //return CreatedAtRoute(nameof(TimesheetController.GetTimecards),new { ids = idsAsString }, timecardModels.ToViewModels());
        //}

        // PUT api/<TimesheetController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TimesheetController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpOptions]
        public IActionResult GetTimesheetOptions()
        {
            Response.Headers.Add("Allow", string.Join(',',"GET", "OPTIONS", "POST","DELETE"));
            
            // provide response body(not covered by http standard)
            return Ok();
        }
    }
}
