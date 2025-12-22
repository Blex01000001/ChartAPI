using System.Data.SQLite;
using ChartAPI.DataAccess.Interfaces;
using System.Data;
using System.Reflection;
using ChartAPI.Domain.Entities;

namespace ChartAPI.DataAccess.SQLite.Repositories
{
    public class ManHourRepository : BaseRepository<ManHour>, IManHourRepository
    {
        public ManHourRepository(IConfiguration config)
            : base(config, tableName: "ManHour", dbFileName: "ManHourData.db")
        {

        }
        public override Task InsertAsync(IEnumerable<ManHour> models)
        {
            const int BatchSize = 10000; // 每批最大處理筆數
            PropertyInfo[] props = typeof(ManHour).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string columns = string.Join(", ", props.Select(p => p.Name));
            string paramList = string.Join(", ", props.Select(p => "@" + p.Name));

            string insertSql = $"INSERT INTO ManHour ({columns}) VALUES ({paramList})";

            using (var conn = CreateConnection())
            {
                conn.Open();
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
            }
            return Task.CompletedTask;
        }
        public override Task DeleteAsync(IEnumerable<ManHour> models)
        {
            var keysToDelete = models
                .Select(m => new { m.Name, m.ID, m.Weekend })
                .Distinct()
                .ToList();
            if (keysToDelete.Count == 0) return Task.CompletedTask;

            string deleteSql = "DELETE FROM ManHour WHERE Name=@Name AND ID=@ID AND Weekend=@Weekend";

            using (var conn = CreateConnection())
            {
                conn.Open();
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
            }
            return Task.CompletedTask;
        }
    }
}
