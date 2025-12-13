using ChartAPI.Extensions;
using ChartAPI.Interfaces;
using ChartAPI.Models;
using HtmlAgilityPack;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ChartAPI.Models.Filters;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.DataAccess.SQLite.Utilities;

namespace ChartAPI.Repositories
{
    public class SQLiteRepository : IDataRepository
    {
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _propertyCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        private string _dBFileName = "ManHourData.db";
        private readonly string _dataBaseDir;
        private readonly Materializer _materializer = new Materializer();
        private string dataBaseFilePath;
        public SQLiteRepository(IConfiguration config)
        {
            _dataBaseDir = config.GetConnectionString("DataBaseDir");
            dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);
        }
        public async Task UpsertData(IFilter filter, string tableName)
        {
            IEnumerable<EmployeeModel> employees = GetData<EmployeeModel>(filter, tableName);
            foreach (EmployeeModel employee in employees)
            {
                string filePath = await DownLoadHtmlTable(employee);
                List<ManHourModel> manHourModels = ParseHtmlTable(filePath, employee.employee_name, employee.employee_id);
                UpdateToDataBase(manHourModels);
            }
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
        public async void BatchUpsertData(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.xls");
            List<ManHourModel> manHourModels = await ParseHtmlTableAsync(files);
            UpdateToDataBase(manHourModels);
        }
        private async Task<List<ManHourModel>> ParseHtmlTableAsync(string[] files)
        {
            var allRecords = new ConcurrentBag<ManHourModel>();
            await Parallel.ForEachAsync(files, async (file, token) =>
            {
                try
                {
                    ConsoleExtensions.WriteLineWithTime($"正在處理檔案：{Path.GetFileName(file)}");

                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                    var dashIndex = fileNameWithoutExt.IndexOf('-');
                    string id = fileNameWithoutExt.Substring(0, dashIndex);// "-" 前
                    string name = fileNameWithoutExt.Substring(dashIndex + 1);// "-" 後

                    var records = ParseHtmlTable(file, name, id);

                    foreach (var r in records)
                        allRecords.Add(r);

                    ConsoleExtensions.WriteLineWithTime($"  -> 匯入 {records.Count} 筆");
                }
                catch (Exception ex)
                {
                    ConsoleExtensions.WriteLineWithTime($"  [錯誤] {Path.GetFileName(file)}: {ex.Message}");
                }
            });
            return allRecords.ToList();
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
                            WorkNo = cells[1].InnerText.Trim() == ""? cells[4].InnerText.Trim() : cells[1].InnerText.Trim(),
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
                            Regular = j == 0? true : false,
                            Overtime = j == 1 ? true : false
                        };
                        result.Add(record);
                    }
                }
            }
            return result;
        }
        private void UpdateToDataBase(List<ManHourModel> manHourModels)
        {
            const int BatchSize = 10000; // 每批最大處理筆數
            string dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);

            using (var conn = new SQLiteConnection($"Data Source={dataBaseFilePath}"))
            {
                conn.Open();

                // 建表
                string createTable = @"
                    CREATE TABLE IF NOT EXISTS ManHour (
                    Name TEXT, ID TEXT, Date TEXT, Year INTEGER, Month INTEGER, Weekend TEXT, WorkNo TEXT,
                    PH TEXT, DP TEXT, CostCode TEXT, CL TEXT, UnitArea TEXT, SystemNo TEXT, EquipNo TEXT,
                    SE TEXT, REWK TEXT, DayofWeek INTEGER, Hours REAL, Regular INTEGER, Overtime INTEGER,
                    Updated TEXT, Position TEXT, Group1 TEXT, Group2 TEXT, Group3 TEXT, Group4 TEXT)";
                new SQLiteCommand(createTable, conn).ExecuteNonQuery();

                // 建立索引
                string createINDEX = @"
                    CREATE INDEX IF NOT EXISTS IX_ManHour_NameIDWeekend ON ManHour(Name, ID, Weekend);";
                new SQLiteCommand(createINDEX, conn).ExecuteNonQuery();


                using (var tran = conn.BeginTransaction())
                {
                    DeleteObsolete(conn, tran, manHourModels);
                    #region
                    // ============ (1) 批次刪除 ============ 
                    //var keysToDelete = manHourModels
                    //    .Select(m => new { m.Name, m.ID, m.Weekend })
                    //    .Distinct()
                    //    .ToList();

                    //if (keysToDelete.Count > 0)
                    //{
                    //    string deleteSql = "DELETE FROM ManHour WHERE Name=@Name AND ID=@ID AND Weekend=@Weekend";

                    //    using (var delCmd = new SQLiteCommand(deleteSql, conn, tran))
                    //    {
                    //        var pName = delCmd.Parameters.Add("@Name", DbType.String);
                    //        var pID = delCmd.Parameters.Add("@ID", DbType.String);
                    //        var pWeekend = delCmd.Parameters.Add("@Weekend", DbType.String);

                    //        foreach (var key in keysToDelete)
                    //        {
                    //            pName.Value = key.Name;
                    //            pID.Value = key.ID;
                    //            pWeekend.Value = key.Weekend.ToString("yyyy-MM-dd");

                    //            delCmd.ExecuteNonQuery();
                    //        }
                    //    }
                    //}
                    #endregion
                    InsertLatest(conn, tran, manHourModels);
                    #region
                    // ============ (2) 批次插入 ============ 
                    //PropertyInfo[] props = typeof(ManHourModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    //string columns = string.Join(", ", props.Select(p => p.Name));
                    //string paramList = string.Join(", ", props.Select(p => "@" + p.Name));

                    //string insertSql = $"INSERT INTO ManHour ({columns}) VALUES ({paramList})";

                    //using (var insertCmd = new SQLiteCommand(insertSql, conn, tran))
                    //{
                    //    // 預先建立參數
                    //    foreach (var prop in props)
                    //    {
                    //        insertCmd.Parameters.Add("@" + prop.Name, DbType.String); // 統一用字串，SQLite 會自動轉型
                    //    }

                    //    for (int i = 0; i < manHourModels.Count; i += BatchSize)
                    //    {
                    //        var batch = manHourModels.Skip(i).Take(BatchSize);

                    //        foreach (var model in batch)
                    //        {
                    //            foreach (var prop in props)
                    //            {
                    //                object value = prop.GetValue(model);

                    //                if (value is DateTime dt)
                    //                    insertCmd.Parameters["@" + prop.Name].Value = dt.ToString("yyyy-MM-dd");
                    //                else if (value is bool b)
                    //                    insertCmd.Parameters["@" + prop.Name].Value = b ? 1 : 0;
                    //                else if (value is DayOfWeek dow)
                    //                    insertCmd.Parameters["@" + prop.Name].Value = (int)dow;
                    //                else if (value == null)
                    //                    insertCmd.Parameters["@" + prop.Name].Value = DBNull.Value;
                    //                else
                    //                    insertCmd.Parameters["@" + prop.Name].Value = value;
                    //            }
                    //            insertCmd.ExecuteNonQuery();
                    //        }
                    //    }
                    //}
                    #endregion
                    tran.Commit();
                }
            }
            ConsoleExtensions.WriteLineWithTime($"Update To DataBase Complete");
        }
        private void DeleteObsolete(SQLiteConnection conn, SQLiteTransaction tran, List<ManHourModel> Data)
        {
            // ============ (1) 批次刪除 ============ 
            var keysToDelete = Data
                .Select(m => new { m.Name, m.ID, m.Weekend })
                .Distinct()
                .ToList();

            if (keysToDelete.Count > 0)
            {
                string deleteSql = "DELETE FROM ManHour WHERE Name=@Name AND ID=@ID AND Weekend=@Weekend";

                using (var delCmd = new SQLiteCommand(deleteSql, conn, tran))
                {
                    var pName = delCmd.Parameters.Add("@Name", DbType.String);
                    var pID = delCmd.Parameters.Add("@ID", DbType.String);
                    var pWeekend = delCmd.Parameters.Add("@Weekend", DbType.String);

                    foreach (var key in keysToDelete)
                    {
                        pName.Value = key.Name;
                        pID.Value = key.ID;
                        pWeekend.Value = key.Weekend.ToString("yyyy-MM-dd");

                        delCmd.ExecuteNonQuery();
                    }
                }
            }
        }
        private void InsertLatest(SQLiteConnection conn, SQLiteTransaction tran, List<ManHourModel> Data)
        {
            // ============ (2) 批次插入 ============ 
            const int BatchSize = 10000; // 每批最大處理筆數
            PropertyInfo[] props = typeof(ManHourModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string columns = string.Join(", ", props.Select(p => p.Name));
            string paramList = string.Join(", ", props.Select(p => "@" + p.Name));

            string insertSql = $"INSERT INTO ManHour ({columns}) VALUES ({paramList})";

            using (var insertCmd = new SQLiteCommand(insertSql, conn, tran))
            {
                // 預先建立參數
                foreach (var prop in props)
                {
                    insertCmd.Parameters.Add("@" + prop.Name, DbType.String); // 統一用字串，SQLite 會自動轉型
                }

                for (int i = 0; i < Data.Count; i += BatchSize)
                {
                    var batch = Data.Skip(i).Take(BatchSize);

                    foreach (var model in batch)
                    {
                        foreach (var prop in props)
                        {
                            object value = prop.GetValue(model);

                            if (value is DateTime dt)
                                insertCmd.Parameters["@" + prop.Name].Value = dt.ToString("yyyy-MM-dd");
                            else if (value is bool b)
                                insertCmd.Parameters["@" + prop.Name].Value = b ? 1 : 0;
                            else if (value is DayOfWeek dow)
                                insertCmd.Parameters["@" + prop.Name].Value = (int)dow;
                            else if (value == null)
                                insertCmd.Parameters["@" + prop.Name].Value = DBNull.Value;
                            else
                                insertCmd.Parameters["@" + prop.Name].Value = value;
                        }
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

        }
        public IEnumerable<TModel> GetData<TModel>(IFilter filter, string tableName) where TModel : new()
        {
            var result = new List<TModel>();
            //Stopwatch ExecuteReaderTime = new Stopwatch();
            //Stopwatch AutoMapReaderTime = new Stopwatch();

            //var (sql, ps) = QueryBuilder.Build(tableName, filter);

            //using (var conn = new SQLiteConnection($"Data Source={dataBaseFilePath}"))
            //using (var cmd = new SQLiteCommand(sql, conn))
            //{
            //    cmd.Parameters.AddRange(ps);
            //    conn.Open();
            //    ExecuteReaderTime.Start();
            //    using (var reader = cmd.ExecuteReader())
            //    {
            //        ExecuteReaderTime.Stop();
            //        AutoMapReaderTime.Start();
            //        while (reader.Read())
            //        {
            //            result.Add(_materializer.Map<TModel>(reader));
            //        }
            //        AutoMapReaderTime.Stop();
            //    }
            //}
            //ConsoleExtensions.WriteLineWithTime($"Query Count: {result.Count}, SQL Execute {ExecuteReaderTime.ElapsedMilliseconds} ms, Materializer Elapsed {AutoMapReaderTime.ElapsedMilliseconds} ms");
            return result;
        }

        public SumModel GetSumData(int year, string group2Filter, string reportTitle, bool isOvertime = false, List<string> costCodes = null)
        {
            string dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);

            // 建立一個 SumModel 來收集所有結果
            var resultModel = new SumModel
            {
                Year = year,
                Title = reportTitle, // 使用傳入的標題
                sumItems = new List<SumItem>()
            };

            var sb = new StringBuilder();
            sb.AppendLine(@"
                SELECT 
                    mh.ID, 
                    mh.Name, 
                    SUM(mh.Hours) AS TotalHours,
                    mh.Year
                FROM ManHour AS mh
                LEFT JOIN ""EmpInfo9933"" AS emp
                    ON mh.ID = emp.employee_id
                WHERE 1=1
                ");

            var parameters = new List<SQLiteParameter>();

            // 1. Group2 條件 (like "%管線%")
            if (!string.IsNullOrEmpty(group2Filter))
            {
                sb.AppendLine(" AND emp.Group2 LIKE @Group2Filter ");
                parameters.Add(new SQLiteParameter("@Group2Filter", $"%{group2Filter}%"));
            }

            // 2. Year 條件
            sb.AppendLine(" AND mh.Year = @Year ");
            parameters.Add(new SQLiteParameter("@Year", year));

            // 3. 可替換的動態條件
            if (isOvertime)
            {
                // AND Overtime = 1
                sb.AppendLine(" AND mh.Overtime = 1 ");
            }
            else if (costCodes != null && costCodes.Any())
            {
                // AND CostCode IN ("002", "003", ...)
                var inParams = costCodes
                    .Select((_, idx) => $"@CostCode{idx}")
                    .ToArray();

                sb.AppendLine($" AND mh.CostCode IN ({string.Join(",", inParams)}) ");

                for (int i = 0; i < costCodes.Count; i++)
                {
                    parameters.Add(new SQLiteParameter(inParams[i], costCodes[i]));
                }
            }

            // 必須 GROUP BY 才能對 ID/Name SUM
            sb.AppendLine(@"
                GROUP BY mh.ID, mh.Name, mh.Year
                ORDER BY TotalHours DESC
                ");

            Stopwatch ExecuteReaderTime = new Stopwatch();
            using (var conn = new SQLiteConnection($"Data Source={dataBaseFilePath}"))
            {
                conn.Open();
                var cmd = new SQLiteCommand(sb.ToString(), conn);
                cmd.Parameters.AddRange(parameters.ToArray());

                ExecuteReaderTime.Start();
                var reader = cmd.ExecuteReader();
                ExecuteReaderTime.Stop();

                while (reader.Read())
                {
                    // 將每一筆 GROUP BY 結果，轉換為一個 SumItem
                    var item = new SumItem
                    {
                        ID = reader["ID"].ToString(),
                        Name = reader["Name"].ToString(),
                        SumValue = Convert.ToDouble(reader["TotalHours"])
                    };

                    // 將 SumItem 加入到 SumModel 的 List 中
                    resultModel.sumItems.Add(item);
                }
            }

            ConsoleExtensions.WriteLineWithTime($"Query Count: {resultModel.sumItems.Count}, Elapsed {ExecuteReaderTime.ElapsedMilliseconds} ms");

            return resultModel;
        }
    }
}
