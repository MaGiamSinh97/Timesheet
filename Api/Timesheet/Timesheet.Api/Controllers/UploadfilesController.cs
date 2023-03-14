using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Timesheet.Api.Services;

namespace FileuploadwithReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadfilesController : ControllerBase
    {
        private readonly EmployeeService serviceEmp;
        private readonly TimesheetService serviceTs;
        public UploadfilesController(EmployeeService serviceEmp, TimesheetService serviceTs)
        {
            this.serviceEmp = serviceEmp ?? throw new ArgumentNullException();
            this.serviceTs = serviceTs ?? throw new ArgumentNullException();
        }
        [HttpPost("ImportFile")]
        public async Task<IActionResult> ImportFile([FromForm] IFormFile file)
        {
            string name = file.FileName;
            string extension = Path.GetExtension(file.FileName);
            //read the file
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var workbook = new XLWorkbook(memoryStream))
                {
                    var nonEmptyDataRows = workbook.Worksheet(1).RowsUsed().Skip(1);

                    foreach (var dataRow in nonEmptyDataRows)
                    {
                        Timesheet.Core.Employee employee = new Timesheet.Core.Employee()
                        {
                            AccNo = (int)dataRow.Cell(1).Value.GetNumber(),
                            FullName = dataRow.Cell(2).Value.GetText(),
                            Ldap = dataRow.Cell(3).Value.GetText(),
                            Project = dataRow.Cell(4).Value.GetText(),
                            Du = dataRow.Cell(5).Value.GetText()
                        };
                        if (!serviceEmp.CheckDuplicate(employee.Ldap))
                        {
                            await serviceEmp.Add(employee);
                        }
                        employee = await serviceEmp.FindAsync(employee.Ldap);

                        Timesheet.Core.Timesheet timesheet = new Timesheet.Core.Timesheet()
                        {
                            Date = dataRow.Cell(6).Value.GetDateTime(),
                            TimeIn = dataRow.Cell(7).Value.GetDateTime(),
                            Timeout = dataRow.Cell(8).Value.GetDateTime(),
                            Employee = employee
                        };
                        await serviceTs.Add(timesheet);
                    }

                }
            }
            //do something with the file here

            return Ok();
        }
    }
}