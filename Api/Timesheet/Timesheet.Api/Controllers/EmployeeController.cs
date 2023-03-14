using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Timesheet.Api.Services;
using Timesheet.Api.ViewModels;
using Timesheet.Api.ViewModels.Extensions;

namespace Timesheet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService service;

        public EmployeeController(EmployeeService service)
        {
            this.service = service ?? throw new ArgumentNullException();
        }

        [HttpGet]
        [HttpHead]
        public async Task<ActionResult<Core.Employee[]>> Get()
        {
            var employees = await this.service.GetAllAsync();
            return Ok(employees.ToViewModels());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Core.Employee[]>> Get(int id)
        {
            var employee = await this.service.GetAsync(id);
            if(employee != null)
            {
                return Ok(employee.ToViewModel());
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Core.Employee[]>> Post(Core.Employee model)
        {
            model.Id = 0;
           
            await this.service.Add(model);
            return CreatedAtRoute(nameof(EmployeeController.Get), new { id = model.Id });
        }

        [HttpPut]
        public async Task<ActionResult<Core.Employee[]>> Put(Core.Employee model)
        {
            model.Id = 0;

            await this.service.Add(model);
            return NoContent();
        }

        [HttpPatch]
        public ActionResult<Core.Employee[]> Patch(JsonPatchDocument<UpdateEmployeeViewModel> patchDocument)
        {
            //model.Id = 0;

            //await this.service.Add(model);
            //return CreatedAtRoute(nameof(TaskController.Get), new { id = model.Id });
            var model = new UpdateEmployeeViewModel();
            patchDocument.ApplyTo(model,ModelState);

            if(!TryValidateModel(model))
            {
                return ValidationProblem(ModelState);
            }

            return NoContent();
        }

        [HttpGet("GetTimesheet")]
        public async Task<ActionResult<Core.Employee[]>> GetTimesheet()
        {
            
            var employees = await this.service.GetAllAsync();
            return Ok(employees.ToViewModels());
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            // So we don't need to write a logic, we get the same logic/code from Startup.cs ApiBehaviorOptions
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (options.Value.InvalidModelStateResponseFactory(this.ControllerContext) as ActionResult);
        }
    }
}
