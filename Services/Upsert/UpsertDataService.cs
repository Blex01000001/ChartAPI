using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Extensions;
using ChartAPI.Models;
using ChartAPI.Models.Filters;
using ChartAPI.Services.Queries;
using HtmlAgilityPack;
using System.Text;
using System.Web;

namespace ChartAPI.Services.Upsert
{
    public class UpsertDataService : IUpsertDataService
    {
        private IEmployeeRepository _empRepo;
        private IManHourRepository _manHourRepo;

        public UpsertDataService(IEmployeeRepository _empQuery, IManHourRepository _manHourRepo)
        {
            this._empRepo = _empQuery;
            this._manHourRepo = _manHourRepo;
        }
        async Task IUpsertDataService.UpsertDataAsync(string name = null, string id = null)
        {
            BaseFilter filter = new EmployeeFilter();
            if (!string.IsNullOrWhiteSpace(id))
                filter.Set("employee_id", id);
            if (!string.IsNullOrWhiteSpace(name))
                filter.Set("name", name);

            IEnumerable<EmployeeModel> employees = _empRepo.GetByFilterAsync(filter);
            foreach (EmployeeModel employee in employees)
            {
                string filePath = await DownLoadHtmlTable(employee);
                List<ManHourModel> manHourModels = ParseHtmlTable(filePath, employee.employee_name, employee.employee_id);
                _manHourRepo.UpdateToDataBase(manHourModels);
            }
            return;
        }
        private async Task<string> DownLoadHtmlTable(EmployeeModel employee)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true,  // 使用目前登入的 Windows 憑證
                PreAuthenticate = true,
                AllowAutoRedirect = true
            };

            using var client = new HttpClient(handler);
            string year = DateTime.Today.Year.ToString();
            var uriBuilder = new UriBuilder("https://ctcieip.ctci.com/hr_gmh/HR_GMH_6011_Export.aspx");
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["corp_id"] = "9933";
            query["emp_id"] = employee.employee_id;
            query["begdate"] = $"{year}01";
            query["enddate"] = $"{year}12";
            query["emp_name"] = employee.employee_name;
            uriBuilder.Query = query.ToString();
            try
            {
                //Console.WriteLine($"Try DownLoadHtmlTable, name: { employee.employee_name} id: { employee.employee_id}");
                var fileBytes = await client.GetByteArrayAsync(uriBuilder.ToString());

                // 存檔
                string tempPath = Path.GetTempPath();
                string fileName = Guid.NewGuid().ToString() + ".xls";
                string filePath = Path.Combine(tempPath, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);
                ConsoleExtensions.WriteLineWithTime($"{employee.employee_name} -> {fileName}");
                return filePath;
            }
            catch (HttpRequestException ex)
            {
                ConsoleExtensions.WriteLineWithTime($"** Error:{ex.Message}");
                return ex.Message;
            }
        }
        private List<ManHourModel> ParseHtmlTable(string filePath, string name, string id)
        {
            var result = new List<ManHourModel>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(filePath, Encoding.UTF8);

            var rows = htmlDoc.DocumentNode.SelectNodes("//table//tr");
            if (rows == null) return result;

            foreach (var row in rows.Skip(1)) // 假設第一列是標題
            {
                var cells = row.SelectNodes("td");
                if (cells == null) continue;

                //foreach (var cell in cells)
                //{
                //    Console.Write(cell.InnerText.Trim() + "\t");
                //}

                //開始對每欄解析放到ManHourModel
                DateTime lastDay = DateTime.Parse(cells[0].InnerText.Trim());
                for (int i = 0; i < (int)lastDay.DayOfWeek + 1; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        DateTime date = lastDay.AddDays(-(i));
                        int startColumn = 11 + (2 * (int)date.DayOfWeek);
                        double hour = double.Parse(cells[startColumn + j].InnerText.Trim());
                        if (hour == 0) continue;

                        var record = new ManHourModel()
                        {
                            ID = id,
                            Name = name,
                            Date = date,
                            Year = date.Year,
                            Month = date.Month,
                            Weekend = lastDay,
                            WorkNo = cells[1].InnerText.Trim() == "" ? cells[4].InnerText.Trim() : cells[1].InnerText.Trim(),
                            PH = cells[2].InnerText.Trim(),
                            DP = cells[3].InnerText.Trim(),
                            CostCode = cells[4].InnerText.Trim(),
                            CL = cells[5].InnerText.Trim(),
                            UnitArea = cells[6].InnerText.Trim(),
                            SystemNo = cells[7].InnerText.Trim(),
                            EquipNo = cells[8].InnerText.Trim(),
                            SE = cells[9].InnerText.Trim(),
                            REWK = cells[10].InnerText.Trim(),
                            DayofWeek = date.DayOfWeek,
                            Hours = hour,
                            Updated = DateTime.Now,
                            Regular = j == 0 ? true : false,
                            Overtime = j == 1 ? true : false
                        };
                        result.Add(record);
                    }
                }
            }
            return result;
        }

    }
}
