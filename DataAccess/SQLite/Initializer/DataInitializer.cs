
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Data.SQLite;
using System.Xml.Linq;

namespace ChartAPI.DataAccess.SQLite.Initializer
{
    public class DataInitializer : IDataInitializer
    {
        private readonly string _dataBaseDir;
        protected readonly string _dBFileName;
        private readonly string _dataBaseFilePath;

        public DataInitializer(IConfiguration config)
        {
            // 從 appsettings.json 取 SQLite 位置
            this._dataBaseDir = config.GetConnectionString("DataBaseDir");
            this._dBFileName = "ManHourData.db";
            this._dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);
        }
        public async Task EnsureTablesCreatedAsync()
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            // --- 建立 ManHour 資料表（如不存在） ---
            var manHourTableSql = @"
                CREATE TABLE IF NOT EXISTS ManHour (
                    Name TEXT,
                    ID TEXT,
                    Date TEXT, 
                    Year INTEGER, 
                    Month INTEGER, 
                    Weekend TEXT, 
                    WorkNo TEXT,
                    PH TEXT, 
                    DP TEXT, 
                    CostCode TEXT, 
                    CL TEXT, 
                    UnitArea TEXT, 
                    SystemNo TEXT, 
                    EquipNo TEXT,
                    SE TEXT, 
                    REWK TEXT, 
                    DayofWeek INTEGER, 
                    Hours REAL, 
                    Regular INTEGER, 
                    Overtime INTEGER,
                    Updated TEXT, 
                    Position TEXT, 
                    Group1 TEXT, 
                    Group2 TEXT, 
                    Group3 TEXT, 
                    Group4 TEXT
                );";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = manHourTableSql;
                await cmd.ExecuteNonQueryAsync();
            }

            // 如果你有 Employee 表，在這裡加
            // var employeeTableSql = "...";
        }
        public async Task EnsureIndexesCreatedAsync()
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            // ---- ManHour 資料表索引 ----
            string[] indexSqlList =
            {
                //"CREATE INDEX IF NOT EXISTS IDX_ManHour_ID ON ManHour(ID);",
                "CREATE INDEX IF NOT EXISTS  IDX_Year_Name_ID ON ManHour(year, name, id)",
                "CREATE INDEX IF NOT EXISTS  IDX_Year_ID ON ManHour(year, id)",
                //"CREATE INDEX IF NOT EXISTS IDX_ManHour_WorkDate ON ManHour(WorkDate);",
                "CREATE INDEX IF NOT EXISTS IDX_ManHour_CostCode ON ManHour(CostCode);",
                //"CREATE INDEX IF NOT EXISTS IX_ManHour_NameIDWeekend ON ManHour(Name, ID, Weekend);"
                "CREATE INDEX IF NOT EXISTS IDX_EmpInfo9933_Name ON EmpInfo9933(employee_name);"
            };

            foreach (var sql in indexSqlList)
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync();
            }
        }
        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection($"Data Source={_dataBaseFilePath}");
        }

    }
}
