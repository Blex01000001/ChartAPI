using ChartAPI.Extensions;
using ChartAPI.Interfaces;
using ChartAPI.Models;
using ChartAPI.Services;
using ChartAPI.Services.Chart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace ChartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly IAnnualSummaryService _annualSummaryService;
        private readonly ICalendarSummaryService _calendarSummaryService;
        public ChartController(
            IAnnualSummaryService annualSummaryService,
            ICalendarSummaryService calendarSummaryService
            )
        {
            _annualSummaryService = annualSummaryService;
            _calendarSummaryService = calendarSummaryService;
        }
        [HttpGet("CalendarSummary")]
        public IActionResult GetCalendarSummary([FromQuery] string name, string id = null)
        {
            ConsoleExtensions.WriteLineWithTime($"CalendarSummary: {name} {id}");
            if (string.IsNullOrWhiteSpace(name))
                return Ok(new { success = false, message = "姓名不正確" });
            return Ok(_calendarSummaryService.GetChart(name, id).Result);
        }
        [HttpGet("AnnualSummary")]
        public IActionResult GetAnnualSummary([FromQuery] int? year, string name = null, string id = null)
        {
            ConsoleExtensions.WriteLineWithTime($"AnnualSummary: {year} {name} {id}");
            if (year == null || year < 2012 || year > DateTime.Now.Year)
                return Ok(new { success = false, message = "年份不正確" });
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(id))
                return Ok(new { success = false, message = "姓名或工號擇一填入" });
            return Ok(_annualSummaryService.GetChart(year.Value, name, id));
        }
        //[HttpGet]
        //public IActionResult GetDeptYearChartDto([FromQuery] int? year, string dept)
        //{
        //    ConsoleExtensions.WriteLineWithTime($"{dept}");
        //    if (string.IsNullOrWhiteSpace(dept))
        //        return Ok(new { success = false, message = "dept為空" });
        //    return Ok(_service.GetDeptYearChartDto(dept));
        //}
        //[HttpGet]
        //public async Task<IActionResult> UpsertDataByDept([FromQuery] string dept, string connectionId)
        //{
        //    if (string.IsNullOrWhiteSpace(dept))
        //        return Ok(new { success = false, message = "dept空白" });
        //    ConsoleExtensions.WriteLineWithTime($"dept {dept}");
        //    await _service.UpsertDataByDept(dept, connectionId);
        //    return Ok(new { success = true, message = dept + " UpsertData success"});
        //}
    }
}
