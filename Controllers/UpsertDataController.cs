using ChartAPI.Extensions;
using ChartAPI.Services.Upsert;
using Microsoft.AspNetCore.Mvc;

namespace ChartAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class UpsertDataController : ControllerBase
    {
        private readonly IUpsertDataService _service;

        public UpsertDataController(IUpsertDataService service)
        {
            _service = service;
        }
        /// <summary>
        /// 更新資料（Upsert）
        /// 從前端接收 name 或 id，再由 Service Update。
        /// </summary>
        [HttpPost]
        [Route("Upsert")]
        public IActionResult UpsertData([FromQuery] string name = null, string id = null)
        {
            ConsoleExtensions.WriteLineWithTime($"{name} {id}");
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(id))
                return Ok(new { success = false, message = "姓名或工號擇一填入" });
            _service.UpsertDataAsync(name, id);
            return Ok(new { message = "UpsertData success" });
        }
    }
}
