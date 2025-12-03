using System.Data.SQLite;
using System.Diagnostics;
using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Extensions;
using ChartAPI.Models;
using ChartAPI.Models.Filters;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.DataAccess.SQLite.Utilities;
using System.Data;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChartAPI.DataAccess.SQLite.Repositories
{
    public class ManHourRepository : BaseRepository<ManHourModel>, IManHourRepository
    {
        //private string tableName = "ManHour";
        //private string _dBFileName = "ManHourData.db";
        //private readonly Materializer _materializer = new Materializer();
        //private readonly string _dataBaseDir;
        //private string dataBaseFilePath;

        //public ManHourRepository(IConfiguration config)
        //{
        //    _dataBaseDir = config.GetConnectionString("DataBaseDir");
        //    dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);
        //}
        public ManHourRepository(IConfiguration config)
            : base(config, tableName: "ManHour", dbFileName: "ManHourData.db")
        {

        }
        //IEnumerable<ManHourModel> IRepository<ManHourModel>.GetByFilterAsync(IFilter filter)
        //{
        //    List<ManHourModel> result = new List<ManHourModel>();
        //    Stopwatch ExecuteReaderTime = new Stopwatch();
        //    Stopwatch AutoMapReaderTime = new Stopwatch();

        //    var (sql, ps) = QueryBuilder.Build(tableName, filter);

        //    using (var conn = new SQLiteConnection($"Data Source={dataBaseFilePath}"))
        //    using (var cmd = new SQLiteCommand(sql, conn))
        //    {
        //        cmd.Parameters.AddRange(ps);
        //        conn.Open();
        //        ExecuteReaderTime.Start();
        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            ExecuteReaderTime.Stop();
        //            AutoMapReaderTime.Start();
        //            while (reader.Read())
        //            {
        //                result.Add(_materializer.Map<ManHourModel>(reader));
        //            }
        //            AutoMapReaderTime.Stop();
        //        }
        //    }
        //    ConsoleExtensions.WriteLineWithTime($"Query Count: {result.Count}, SQL Execute {ExecuteReaderTime.ElapsedMilliseconds} ms, Materializer Elapsed {AutoMapReaderTime.ElapsedMilliseconds} ms");
        //    return result;
        //}
        public override Task InsertAsync(IEnumerable<ManHourModel> models)
        {
            const int BatchSize = 10000; // 每批最大處理筆數
            PropertyInfo[] props = typeof(ManHourModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string columns = string.Join(", ", props.Select(p => p.Name));
            string paramList = string.Join(", ", props.Select(p => "@" + p.Name));

            string insertSql = $"INSERT INTO ManHour ({columns}) VALUES ({paramList})";

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            using (var insertCmd = new SQLiteCommand(insertSql, conn, tran))
            {
                // 預先建立參數
                foreach (var prop in props)
                {
                    insertCmd.Parameters.Add("@" + prop.Name, DbType.String); // 統一用字串，SQLite 會自動轉型
                }

                for (int i = 0; i < models.Count(); i += BatchSize)
                {
                    var batch = models.Skip(i).Take(BatchSize);

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
                tran.Commit();
            }
            return Task.CompletedTask;
        }
        public override Task DeleteAsync(IEnumerable<ManHourModel> models)
        {
            var keysToDelete = models
                .Select(m => new { m.Name, m.ID, m.Weekend })
                .Distinct()
                .ToList();
            if (keysToDelete.Count == 0) return Task.CompletedTask;

            string deleteSql = "DELETE FROM ManHour WHERE Name=@Name AND ID=@ID AND Weekend=@Weekend";

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
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
                tran.Commit();
            }
            return Task.CompletedTask;
        }
        void IManHourRepository.UpdateToDataBase(List<ManHourModel> manHourModels)
        {
            const int BatchSize = 10000; // 每批最大處理筆數
            using (var conn = CreateConnection())
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
                    InsertLatest(conn, tran, manHourModels);
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

    }
}
