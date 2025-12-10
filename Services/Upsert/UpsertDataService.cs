using ChartAPI.DataAccess.Interfaces;
using ChartAPI.DataAccess.SQLite.Initializer;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.Extensions;
using ChartAPI.Hubs;
using ChartAPI.Models;
using ChartAPI.Models.Filters;
using ChartAPI.Services.Queries;
using HtmlAgilityPack;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.Web;

namespace ChartAPI.Services.Upsert
{
    public class UpsertDataService : IUpsertDataService
    {
        private readonly IDataInitializer _initializer;
        private readonly IEmployeeRepository _empRepo;
        private readonly IManHourRepository _manHourRepo;
        private readonly IHubContext<NotifyHub> _hubContext;

        public UpsertDataService(
            IDataInitializer initializer,
            IEmployeeRepository empQuery, 
            IManHourRepository manHourRepo, 
            IHubContext<NotifyHub> hubContext)
        {
            this._initializer = initializer;
            this._empRepo = empQuery;
            this._manHourRepo = manHourRepo;
            this._hubContext = hubContext;
        }
        async Task IUpsertDataService.UpsertDataAsync(string name = null, string id = null)
        {
            // 1. 設定filter
            //BaseFilter filter = new EmployeeFilter();
            //if (!string.IsNullOrWhiteSpace(id))
            //    filter.Set("employee_id", id);
            //if (!string.IsNullOrWhiteSpace(name))
            //    filter.Set("employee_name", name);
            var qb = new QueryBuilder<EmployeeModel>("EmpInfo9933")
                .Where(x => x.id == id)
                .Where(x => x.employee_name == name);
            ConsoleExtensions.WriteLineWithTime($"name:{name} id:{id}");

            // 2.撈員工資料
            //IEnumerable<EmployeeModel> employees = _empRepo.GetByFilterAsync(filter);
            IEnumerable<EmployeeModel> employees = _empRepo.GetByQBAsync(qb);

            foreach (EmployeeModel employee in employees)
            {
                // 3.下載最新員工資料
                string filePath = await DownLoadHtmlTable(employee);
                // 4.轉換資料
                List<ManHourModel> manHourModels = ParseHtmlTable(filePath, employee.employee_name, employee.employee_id);
                // 5.更新資料到database
                //  5.1 建表 index
                await _initializer.EnsureTablesCreatedAsync();
                await _initializer.EnsureIndexesCreatedAsync();
                //  5.2 刪除舊資料
                await _manHourRepo.DeleteAsync(manHourModels);
                //  5.3 插入資料
                await _manHourRepo.InsertAsync(manHourModels);
                //_manHourRepo.UpdateToDataBase(manHourModels);
            }
            //await _hubContext.Clients.All.SendAsync("UpsertCompleted", "Completed");
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
