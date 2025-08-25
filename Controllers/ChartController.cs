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
        public IActionResult GetMonthlyData([FromQuery] string name, int year, string id = null)
        {
            Console.WriteLine($"Query: {name} {year} {id}");
            return Ok(_service.GetMonthlyData(name, year, id));
        }
    }
}
