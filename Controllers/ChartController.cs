using ChartAPI.Extensions;
using ChartAPI.Services.Chart;
using Microsoft.AspNetCore.Mvc;

namespace ChartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly ILogger<ChartController> _logger;
        private readonly IAnnualSummaryService _annualSummaryService;
        private readonly ICalendarSummaryService _calendarSummaryService;
        private readonly IDepartmentSummaryService _departmentSummaryService;
        public ChartController(
            ILogger<ChartController> logger,
            IAnnualSummaryService annualSummaryService,
            ICalendarSummaryService calendarSummaryService,
            IDepartmentSummaryService departmentSummaryService
            )
        {
            _logger = logger;
            _annualSummaryService = annualSummaryService;
            _calendarSummaryService = calendarSummaryService;
            _departmentSummaryService = departmentSummaryService;
        }
        [HttpGet("CalendarSummary")]
        public IActionResult GetCalendarSummary([FromQuery] string name, string id = null)
        {
            ConsoleExtensions.WriteLineWithTime($"GetCalendarSummary: name={name} id={id}");
            if (string.IsNullOrWhiteSpace(name))
                return Ok(new { success = false, message = "姓名不正確" });
            return Ok(_calendarSummaryService.GetChart(name, id).Result);
        }
        [HttpGet("AnnualSummary")]
        public IActionResult GetAnnualSummary([FromQuery] int? year, string name = null, string id = null)
        {
            ConsoleExtensions.WriteLineWithTime($"GetAnnualSummary year={year} name={name} id={id}");
            if (year == null || year < 2012 || year > DateTime.Now.Year)
                return Ok(new { success = false, message = "年份不正確" });
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(id))
                return Ok(new { success = false, message = "姓名或工號擇一填入" });

            return Ok(_annualSummaryService.GetChart(year.Value, name, id));
        }
        [HttpGet("DepartmentSummary")]
        public IActionResult GetDepartmentSummary([FromQuery] int? year, string deptName)
        {
            ConsoleExtensions.WriteLineWithTime($"GetDepartmentSummary Dept={deptName}");
            if (year == null)
                return Ok(new { success = false, message = "年份為空" });
            if (string.IsNullOrWhiteSpace(deptName))
                return Ok(new { success = false, message = "deptName為空" });

            return Ok(_departmentSummaryService.GetChart(year.Value, deptName));
        }
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
