using ChartAPI.Interfaces;
using ChartAPI.Models;
using System.Collections;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using HtmlAgilityPack;
using System.Data;
using System.Collections.Concurrent;

namespace ChartAPI.Repositories
{
    public class SQLiteRepository : IDataRepository
    {
        private string _dBFileName = "ManHourData.db";
        private readonly string _dataBaseDir;
        public SQLiteRepository(IConfiguration config)
        {
            _dataBaseDir = config.GetConnectionString("DataBaseDir");
        }
        public void UpsertData(EmployeeFilter filter, string tableName)
        {
            IEnumerable<EmployeeModel> employees = GetData<EmployeeModel, EmployeeFilter>(filter, tableName);
            foreach (EmployeeModel employee in employees)
            {
                string filePath = DownLoadHtmlTable(employee);
                List<ManHourModel> manHourModels = ParseHtmlTable(filePath);
                UpdateToDataBase(manHourModels);
            }

        }
        private string DownLoadHtmlTable(EmployeeModel employee)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true,  // 使用目前登入的 Windows 憑證
                PreAuthenticate = true,
                AllowAutoRedirect = true
            };

            using var client = new HttpClient(handler);
            string year = DateTime.Today.Year.ToString();
            string exportUrl = $"https://ctcieip.ctci.com/hr_gmh/HR_GMH_6061_Export.aspx?corp_id=9933&emp_id={employee.employee_id}&begdate={year}01&enddate={year}12&name={employee.employee_name}";

            try
            {
                var fileBytes = client.GetByteArrayAsync(exportUrl);

                // 存檔
                string tempPath = Path.GetTempPath();
                string fileName = Guid.NewGuid().ToString() + ".txt";
                string filePath = Path.Combine(tempPath, fileName);
                System.IO.File.WriteAllBytesAsync(filePath, fileBytes.Result);
                //Console.WriteLine("下載完成: " + filePath);
                return filePath;
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"下載失敗: {ex.Message}");
                return ex.Message;
            }
        }
        private async Task<List<ManHourModel>> ParseHtmlTableAsync(string[] files)
        {
            var allRecords = new ConcurrentBag<ManHourModel>();
            Parallel.ForEach(files, async (file, token) =>
            {
                try
                {
                    Console.WriteLine($"正在處理檔案：{Path.GetFileName(file)}");
                    var records = ParseHtmlTable(file);

                    foreach (var r in records)
                        allRecords.Add(r);

                    Console.WriteLine($"  -> 匯入 {records.Count} 筆");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [錯誤] {Path.GetFileName(file)}: {ex.Message}");
                }
            });
            return allRecords.ToList();
        }
        private List<ManHourModel> ParseHtmlTable(string filePath)
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

                var record = new ManHourModel
                {
                    Date = DateTime.TryParse(cells[0].InnerText.Trim(), out var dt) ? dt : DateTime.MinValue,
                    WorkNo = cells[1].InnerText.Trim(),
                    CostCode = cells[4].InnerText.Trim(),
                    EquipNo = cells[8].InnerText.Trim(),
                };
                result.Add(record);
            }
            return result;
        }
        private void UpdateToDataBase(List<ManHourModel> manHourModels)
        {
            const int BatchSize = 10000; // 每批處理筆數
            string dbPath = "Data Source=ManHourData.db;Version=3;";

            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();

                // 建表
                Console.Write($"Create Table...");
                string createTable = @"
                    CREATE TABLE IF NOT EXISTS ManHour (
                    Name TEXT, ID TEXT, Date TEXT, Year INTEGER, Month INTEGER, Weekend TEXT, WorkNo TEXT,
                    PH TEXT, DP TEXT, CostCode TEXT, CL TEXT, UnitArea TEXT, SystemNo TEXT, EquipNo TEXT,
                    SE TEXT, REWK TEXT, DayofWeek INTEGER, Hours REAL, Regular INTEGER, Overtime INTEGER,
                    Updated TEXT, Position TEXT, Group1 TEXT, Group2 TEXT, Group3 TEXT, Group4 TEXT)";
                new SQLiteCommand(createTable, conn).ExecuteNonQuery();
                Console.Write($" Complete " + DateTime.Now + "\n");

                // 索引
                Console.Write($"Create IX_ManHour_NameIDWeekend...");
                string createINDEX = @"
                    CREATE INDEX IF NOT EXISTS IX_ManHour_NameIDWeekend
                    ON ManHour(Name, ID, Weekend);";
                new SQLiteCommand(createINDEX, conn).ExecuteNonQuery();
                Console.Write($" Complete " + DateTime.Now + "\n");

                var props = typeof(ManHourModel)
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                using (var tran = conn.BeginTransaction())
                {
                    // ============ (1) 批次刪除 ============ 
                    var keysToDelete = manHourModels
                        .Select(m => new { m.Name, m.ID, m.Weekend })
                        .Distinct()
                        .ToList();

                    if (keysToDelete.Count > 0)
                    {
                        Console.WriteLine($"keysToDelete Count: {keysToDelete.Count}");
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

                    // ============ (2) 批次插入 ============ 
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

                        for (int i = 0; i < manHourModels.Count; i += BatchSize)
                        {
                            var batch = manHourModels.Skip(i).Take(BatchSize);

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
                    tran.Commit();
                }
            }
            Console.Write($"Complete\n");
        }
        public IEnumerable<TModel> GetData<TModel,TFilter>(TFilter filter, string tableName) 
            where TModel : new() 
        {
            var list = new List<TModel>();
            string dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);

            Stopwatch ExecuteReaderTime = new Stopwatch();
            using (var conn = new SQLiteConnection($"Data Source={dataBaseFilePath}"))
            {
                conn.Open();
                var (sql, ps) = BuildFilterSql(filter, tableName);
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddRange(ps);
                ExecuteReaderTime.Start();
                var reader = cmd.ExecuteReader();
                ExecuteReaderTime.Stop();
                var props = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                while (reader.Read())
                {
                    // map reader 到 ManHourModel …
                    var model = new TModel();
                    foreach (var prop in props)
                    {
                        if (reader[prop.Name] == DBNull.Value) continue;

                        var value = reader[prop.Name];

                        if (prop.PropertyType == typeof(DateTime))
                            prop.SetValue(model, DateTime.Parse(value.ToString()));
                        else if (prop.PropertyType == typeof(bool))
                            prop.SetValue(model, Convert.ToInt32(value) == 1);
                        else if (prop.PropertyType == typeof(DayOfWeek))
                            prop.SetValue(model, (DayOfWeek)Convert.ToInt32(value));
                        else if (prop.PropertyType == typeof(int))
                            prop.SetValue(model, Convert.ToInt32(value));
                        else if (prop.PropertyType == typeof(double))
                            prop.SetValue(model, Convert.ToDouble(value));
                        else
                            prop.SetValue(model, value);
                    }
                    list.Add(model);
                }
            }
            Console.Write($"count: {list.Count}, Elapsed {ExecuteReaderTime.ElapsedMilliseconds} ms\n" );
            return list;
        }

        public static (string Sql, SQLiteParameter[] Params) BuildFilterSql<T>(T filter, string tableName)
        {
            var sb = new StringBuilder($"SELECT * FROM {tableName} WHERE 1=1");
            var parameters = new List<SQLiteParameter>();

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var value = prop.GetValue(filter);
                if (value == null)
                    continue;

                var propName = prop.Name;
                var colName = propName;   // 預設欄位名稱同屬性名稱
                var paramBase = "@" + propName;

                // 1) 處理 List<T> → IN (...)
                if (prop.PropertyType.IsGenericType
                    && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var list = ((IEnumerable)value).Cast<object>().ToArray();
                    if (!list.Any()) continue;

                    // 產生參數名稱如 @Name0,@Name1,...
                    var inParams = list
                        .Select((_, idx) => $"{paramBase}{idx}")
                        .ToArray();

                    sb.Append($" AND {colName} IN ({string.Join(",", inParams)})");

                    for (int i = 0; i < list.Length; i++)
                    {
                        parameters.Add(new SQLiteParameter(inParams[i], list[i] ?? DBNull.Value));
                    }
                }
                // 2) 處理 DateFrom / DateTo → >= / <=
                else if (propName.EndsWith("From", StringComparison.OrdinalIgnoreCase)
                      || propName.EndsWith("To", StringComparison.OrdinalIgnoreCase))
                {
                    var baseName = propName.EndsWith("From", StringComparison.OrdinalIgnoreCase)
                                 ? propName.Substring(0, propName.Length - 4)
                                 : propName.Substring(0, propName.Length - 2);
                    var op = propName.EndsWith("From", StringComparison.OrdinalIgnoreCase) ? ">=" : "<=";
                    sb.Append($" AND {baseName} {op} {paramBase}");
                    parameters.Add(new SQLiteParameter(paramBase, value));
                }
                // 3) 處理字串 → =
                else if (prop.PropertyType == typeof(string))
                {
                    var s = ((string)value).Trim();
                    if (string.IsNullOrEmpty(s)) continue;

                    sb.Append($" AND {colName} = {paramBase}");
                    parameters.Add(new SQLiteParameter(paramBase, s));
                }
                // 4) 處理值型別（int, bool, DayOfWeek, double, DateTime, Nullable<>... 等）
                else if (prop.PropertyType.IsValueType)
                {
                    sb.Append($" AND {colName} = {paramBase}");
                    parameters.Add(new SQLiteParameter(paramBase, value));
                }
                // 5) 其他型別可自行擴充…
            }

            return (sb.ToString(), parameters.ToArray());
        }
    }
}
