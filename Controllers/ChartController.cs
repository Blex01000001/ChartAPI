using ChartAPI.Extensions;
using ChartAPI.Interfaces;
using ChartAPI.Models;
using ChartAPI.Services;
using ChartAPI.Services.Chart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace ChartAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly IAnnualSummaryService _service;
        public ChartController(IAnnualSummaryService service)
        {
            _service = service;
        }
        //[HttpGet]
        //public IActionResult GetCalendarChart([FromQuery] string name, string id = null)
        //{
        //    return Ok(_service.GetCalendarData(name, id));
        //}
        //[HttpGet]
        //public IActionResult UpsertData([FromQuery] string name = null, string id = null)
        //{
        //    if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(id))
        //        return Ok(new { success = false, message = "姓名或工號擇一填入" });
        //    _service.UpsertData(name, id);
        //    return Ok(new { message = "UpsertData success" });
        //}

        [HttpGet]
        public IActionResult GetAnnualSummary([FromQuery] int? year, string name = null, string id = null)
        {

            ConsoleExtensions.WriteLineWithTime($"{year} {name} {id}");
            if (year == null || year < 2012 || year > DateTime.Now.Year)
                return Ok(new { success = false, message = "年份不正確" });
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(id))
                return Ok(new { success = false, message = "姓名或工號擇一填入" });
            return Ok(_service.GetAnnualSummary(year.Value, name, id));
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
        //[HttpGet]
        //public IActionResult SendEmail(string toEmail, string subject, string body)
        //{
        //    try
        //    {
        //        var mailMessage = new MailMessage();
        //        mailMessage.From = new MailAddress("max.liao@ctci.com");
        //        mailMessage.To.Add("brian.hsieh@ctci.com");
        //        mailMessage.CC.Add("max.liao@ctci.com");
        //        mailMessage.Subject = "test email Subject";
        //        mailMessage.Body = "test email";
        //        mailMessage.IsBodyHtml = true; // 如果內容為HTML

        //        using (var smtpClient = new SmtpClient("smtp.office365.com", 587))
        //        {
        //            smtpClient.Credentials = new NetworkCredential("max.liao@ctci.com", "pw");
        //            smtpClient.EnableSsl = true;
        //            smtpClient.Send(mailMessage);
        //        }

        //        return Ok("郵件已發送");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("發送郵件失敗: " + ex.Message);
        //    }

        //}

    }
}
