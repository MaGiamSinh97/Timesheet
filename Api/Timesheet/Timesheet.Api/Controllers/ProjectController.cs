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
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService service;

        public ProjectController(ProjectService service)
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
                var projects = await this.service.GetAllAsync();
                return Ok(projects.ToViewModels());
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
            var project = await this.service.GetAsync(id);
            if (await this.service.GetAsync(id) != null)
            {
                return Ok(project.ToViewModel());
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Core.Project[]>> Post(Core.Project model)
        {
            model.Id = 0;
            var result = await this.service.Add(model);
            if(result > 0)
            {
                return Ok("Added Successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
            }
        }

        // PUT api/<TimesheetController>/5
        [HttpPut]
        public async Task<ActionResult<Core.Project[]>> Put(Core.Project model)
        {
            var result = await this.service.Update(model);
            if (result > 0)
            {
                return Ok("Updated Successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
            }
        }

        // DELETE api/<TimesheetController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Core.Project[]>> Delete(int Id)
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

        [HttpGet("GetMember/{id}")]
        public async Task<ActionResult<Core.Employee[]>> GetMember(int id)
        {
            var employees = await this.service.GetMember(id);
            return Ok(employees);
        }

        [HttpDelete("RemoveMember/{proId}/{empId}")]
        public async Task<ActionResult<Core.Project[]>> RemoveMember(int proId, int empId)
        {
            var result = await this.service.RemoveMember(proId, empId);
            if (result > 0)
            {
                return Ok("Removed Successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
            }
        }
        [HttpPost("AddMember")]
        public async Task<ActionResult<Core.Project[]>> AddMember(ProjectEmployee projectEmployee)
        {
            var result = await this.service.AddMember(projectEmployee);
            if (result > 0)
            {
                return Ok("Add Member Successfully");
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
