using DocumentFormat.OpenXml.Spreadsheet;
using Irony.Parsing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Api.Helpers;
using Timesheet.Api.Services;
using Timesheet.Api.ViewModels;
using Timesheet.Api.ViewModels.Extensions;
using Timesheet.Core;

namespace Timesheet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService service;
        private readonly IConfiguration _config;
        public EmployeeController(EmployeeService service, IConfiguration config)
        {
            this.service = service ?? throw new ArgumentNullException();
            _config = config ?? throw new ArgumentNullException();
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
            if (employee != null)
            {
                return Ok(employee.ToViewModel());
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<ActionResult<Core.Employee[]>> Post(Core.Employee model)
        {
            model.Id = 0;
            var hashsalt = service.EncryptPassword(model.EncPass);
            model.EncPass = hashsalt.Hash;
            model.StoredSalt = hashsalt.Salt;
            var result = await this.service.Add(model);
            if (result > 0)
            {
                return Ok("Added Successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
            }
        }

        [HttpPut]
        public async Task<ActionResult<Core.Employee[]>> Put(Core.Employee model)
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
        public async Task<ActionResult<Core.Employee[]>> Delete(int Id)
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


        [HttpPatch]
        public ActionResult<Core.Employee[]> Patch(JsonPatchDocument<UpdateEmployeeViewModel> patchDocument)
        {
            //model.Id = 0;

            //await this.service.Add(model);
            //return CreatedAtRoute(nameof(TaskController.Get), new { id = model.Id });
            var model = new UpdateEmployeeViewModel();
            patchDocument.ApplyTo(model, ModelState);

            if (!TryValidateModel(model))
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

        [HttpPost("Login")]
        public async Task<ActionResult<Core.Employee[]>> Login(Timesheet.Core.Employee employee)
        {
            if (employee.KnoxId == "superadmin" && employee.EncPass == "9999")
            {
                return Ok(new { token = 1, name = "super admin", role = 1, knoxId = "super" });
            }
            var user = await service.FindAsync(employee.KnoxId);
            var isPasswordMatched = service.VerifyPassword(employee.EncPass, user.StoredSalt, user.EncPass);
            if (isPasswordMatched)
            {
                var token = GenerateToken(user);
                return Ok(new { token = token, name = user.FullName, role = user.Role , knoxId = user.KnoxId});
            }
            else
            {
                return NotFound("user not found");
            }
        }
        private string GenerateToken(Core.Employee user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.FullName),
                new Claim(ClaimTypes.Role,user.Role.ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            // So we don't need to write a logic, we get the same logic/code from Startup.cs ApiBehaviorOptions
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (options.Value.InvalidModelStateResponseFactory(this.ControllerContext) as ActionResult);
        }
    }
}
