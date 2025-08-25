using ChartAPI.Interfaces;
using ChartAPI.Models;
using System.Collections;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ChartAPI.Repositories
{
    public class SQLiteRepository : IDataRepository
    {
        string DBName = "ManHourData.db";
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        public IEnumerable<ManHourModel> GetManHourData(ManHourFilter filter)
        {
            var list = new List<ManHourModel>();
            string tableName = "ManHour";
            string DBFilePath = Path.Combine("D:\\Code\\C#\\ManHour Analysis\\bin\\Debug", DBName);

            Stopwatch ExecuteReaderTime = new Stopwatch();
            using (var conn = new SQLiteConnection($"Data Source={DBFilePath}"))
            {
                conn.Open();
                var (sql, ps) = BuildFilterSql(filter, tableName);
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddRange(ps);
                ExecuteReaderTime.Start();
                var reader = cmd.ExecuteReader();
                ExecuteReaderTime.Stop();
                var props = typeof(ManHourModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                while (reader.Read())
                {
                    // map reader 到 ManHourModel …
                    var model = new ManHourModel();
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
            Console.WriteLine($"count: {list.Count}, Elapsed {ExecuteReaderTime.ElapsedMilliseconds} ms\n" );
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
