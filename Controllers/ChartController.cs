using ChartAPI.Extensions;
using ChartAPI.Interfaces;
using ChartAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection.Emit;

namespace ChartAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly IChartServices _service;
        public ChartController(IChartServices service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult GetCalendarChart([FromQuery] string name, string id = null)
        {
            return Ok(_service.GetCalendarData(name, id));
        }
        [HttpGet]
        public IActionResult UpsertData([FromQuery] string name = null, string id = null)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(id))
                return Ok(new { success = false, message = "姓名或工號擇一填入" });
            _service.UpsertData(name, id);
            return Ok(new { message = "UpsertData success" });
        }

        [HttpGet]
        public IActionResult GetDashboardResponseDto([FromQuery] int? year, string name = null, string id = null)
        {
            ConsoleExtensions.WriteLineWithTime($"{year} {name} {id}");
            if (year == null || year < 2012 || year > DateTime.Now.Year)
                return Ok(new { success = false, message = "年份不正確" });
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(id))
                return Ok(new { success = false, message = "姓名或工號擇一填入" });
            return Ok(_service.GetDashboardResponseDto(year.Value, name, id));
        }

    }
}
