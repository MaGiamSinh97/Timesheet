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
using Timesheet.Core;
using Timesheet.Api.ViewModels.Extensions;
using Timesheet.Api.ViewModels;
using Timesheet.Api.Helpers;

namespace FileuploadwithReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadfilesController : ControllerBase
    {
        private readonly EmployeeService serviceEmp;
        private readonly TimesheetService serviceTs;
        private readonly ProjectService servicePj;
        private readonly ProjectEmployeeService servicePE;
        public UploadfilesController(EmployeeService serviceEmp, TimesheetService serviceTs, ProjectService servicePj, ProjectEmployeeService servicePE)
        {
            this.serviceEmp = serviceEmp ?? throw new ArgumentNullException();
            this.serviceTs = serviceTs ?? throw new ArgumentNullException();
            this.servicePj = servicePj ?? throw new ArgumentNullException();
            this.servicePE = servicePE ?? throw new ArgumentNullException();
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
                        Project project = new Project()
                        {
                            Du = dataRow.Cell(5).IsEmpty() ? "" : dataRow.Cell(5).Value.GetText(),
                            Name = dataRow.Cell(4).IsEmpty() ? "" : dataRow.Cell(4).Value.GetText(),
                        };
                        if (!servicePj.CheckDuplicate(project.Name))
                        {
                            await servicePj.Add(project);
                        }
                        project = await servicePj.FindAsync(project.Name);

                        Timesheet.Core.Employee employee = new Timesheet.Core.Employee()
                        {
                            AccNo = (int)dataRow.Cell(1).Value.GetNumber(),
                            FullName = dataRow.Cell(2).IsEmpty() ? "" : dataRow.Cell(2).Value.GetText(),
                            KnoxId = dataRow.Cell(3).IsEmpty() ? "" : dataRow.Cell(3).Value.GetText(),
                            Du = dataRow.Cell(5).IsEmpty() ? "" : dataRow.Cell(5).Value.GetText()
                        };
                        if (!serviceEmp.CheckDuplicate(employee.KnoxId))
                        {
                            await serviceEmp.Add(employee);
                        }
                        employee = await serviceEmp.FindAsync(employee.KnoxId);

                        ProjectEmployee projectEmployee = new ProjectEmployee()
                        {
                            Project = project,
                            Employee = employee,
                        };
                        if (!servicePE.CheckDuplicate(project.Id, employee.Id))
                        {
                            await servicePE.Add(projectEmployee);
                        }

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

        [HttpGet("ExportFile/{id}")]
        public async Task<IActionResult> ExportFile(int id)
        {
            using (var workbook = new XLWorkbook())
            {
                var project = await servicePj.GetAsync(id);
                var projectName = "";
                if (id > 0)
                {
                    projectName = project.Name;
                }
                var listDate = DateTimeExtension.GetDates(2023, 3);
                var tsview = new List<TimesheetViewExcelModel>();
                var timesheets = await serviceTs.GetAllByDateRange(listDate[0], listDate[listDate.Count - 1]);
                if (id > 0)
                {
                    tsview = timesheets.ToExcelViewModels().Where(x => x.ProjectId == id).OrderBy(x => x.KnoxId).ToList();
                }
                else
                {
                    tsview = timesheets.ToExcelViewModels().OrderBy(x => x.KnoxId).ToList();
                }

                foreach(var item in tsview)
                {
                    var data = tsview.Where(x=>x.EmployeeId == item.EmployeeId).ToList();
                    item.TotalAbsentTime = data.Sum(d => d.AbsentTime);
                }

                var worksheet = workbook.Worksheets.Add("Timesheet");
                worksheet.Columns().AdjustToContents();
                worksheet.Rows().AdjustToContents();
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "STT";
                worksheet.Cell(currentRow, 2).Value = "Full name";
                worksheet.Cell(currentRow, 3).Value = "DU";
                worksheet.Cell(currentRow, 4).Value = "Project";
                worksheet.Columns("A").Width = 7;
                worksheet.Columns("B").Width = 20;
                worksheet.Columns("C").Width = 7;
                worksheet.Columns("D").Width = 20;
                worksheet.Range("A1:A2").Merge();
                worksheet.Range("B1:B2").Merge();
                worksheet.Range("C1:C2").Merge();
                worksheet.Range("D1:D2").Merge();
                worksheet.Range("A1:A2").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                worksheet.Range("B1:B2").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                worksheet.Range("C1:C2").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                worksheet.Range("D1:D2").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                for (var i = 5; i <= listDate.Count + 4; i++)
                {
                    worksheet.Cell(currentRow, i).Value = listDate[i - 5].Day;
                    worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                    worksheet.Cell(currentRow + 1, i).Value = (listDate[i - 5].DayOfWeek.ToString().Substring(0, 3));
                    worksheet.Cell(currentRow + 1, i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                }
                worksheet.Cell(currentRow, listDate.Count + 5).Value = "MM";
                string lastCol = workbook.Worksheet(1).Row(1).LastCellUsed().Address.ToString();
                worksheet.Range(lastCol + ":" + lastCol.Substring(0, 2) + "2").Merge();
                worksheet.Range(lastCol + ":" + lastCol.Substring(0, 2) + "2").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);

                var row = 2;
                var stt = 0;
                foreach (var item in tsview)
                {
                    if (item.KnoxId != worksheet.Cell(row, 2).Value.ToString())
                    {
                        row++;
                        stt++;
                    }
                    for (var i = 5; i <= listDate.Count + 4; i++)
                    {
                        if (item.Date.Day == (int)worksheet.Cell(currentRow, i).Value)
                        {

                            if (item.AbsentTime > 0)
                            {
                                worksheet.Cell(row, i).Value = item.AbsentTime;
                                worksheet.Cell(row, i).Style.Fill.BackgroundColor = XLColor.Yellow;
                            
                            }
                            worksheet.Cell(row, 1).Value = stt;
                            worksheet.Cell(row, 2).Value = item.KnoxId;
                            worksheet.Cell(row, 3).Value = item.Du;
                            worksheet.Cell(row, 4).Value = item.Project;
                            worksheet.Cell(row, listDate.Count + 5).Value = ManMonth((double)item.TotalAbsentTime);
                            worksheet.Cell(row, listDate.Count + 5).Style.Fill.BackgroundColor = XLColor.Orange;
                        }
                    }

                }

                worksheet.RowsUsed().Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                worksheet.RowsUsed().Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.Black;

                worksheet.RowsUsed().Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                worksheet.RowsUsed().Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.Black;

                worksheet.RowsUsed().Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                worksheet.RowsUsed().Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.Black;

                worksheet.RowsUsed().Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                worksheet.RowsUsed().Style.Border.RightBorderColor = ClosedXML.Excel.XLColor.Black;


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        projectName + "_timesheets.xlsx");
                }
            }
        }


        private static double GetWorkingDays(DateTime from, DateTime to)
        {
            var totalDays = 0;
            for (var date = from; date < to; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday
                    && date.DayOfWeek != DayOfWeek.Sunday)
                    totalDays++;
            }

            return totalDays;
        }

        private static double ManMonth(double absentTime)
        {
            var workingDay = GetWorkingDays(DateTimeExtension.FirstDayOfMonth(DateTime.Now), DateTimeExtension.LastDayOfMonth(DateTime.Now));
            var actualWorking = (workingDay - ((absentTime/60) / 8));
            if(workingDay > 22)
            {
                actualWorking = actualWorking - 1;
            }
            else if(workingDay <= 22)
            {
                actualWorking = actualWorking + (22 - workingDay);
            }
            return actualWorking / 22;
        }

    }
}