using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Models;
using ChartAPI.Models.Filters;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.DataAccess.SQLite.Utilities;
using ChartAPI.Extensions;
using System.Data.SQLite;
using System.Data;
using System.Reflection;

namespace ChartAPI.DataAccess.SQLite.Repositories
{
    public class EmployeeRepository : BaseRepository<EmployeeModel>, IEmployeeRepository
    {
        //private string tableName = "EmpInfo9933";
        //private string _dBFileName = "ManHourData.db";
        //private readonly Materializer _materializer = new Materializer();
        //private readonly string _dataBaseDir;
        //private string dataBaseFilePath;

        //public EmployeeRepository(IConfiguration config)
        //{
        //    _dataBaseDir = config.GetConnectionString("DataBaseDir");
        //    dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);
        //}
        public EmployeeRepository(IConfiguration config)
            : base(config, tableName: "EmpInfo9933", dbFileName: "ManHourData.db")
        {
            
        }

        public override Task DeleteAsync(IEnumerable<EmployeeModel> models)
        {
            throw new NotImplementedException();
        }

        public override Task InsertAsync(IEnumerable<EmployeeModel> models)
        {
            throw new NotImplementedException();
        }
        //IEnumerable<EmployeeModel> IRepository<EmployeeModel>.GetByFilterAsync(IFilter filter)
        //{
        //    List<EmployeeModel> employees = new List<EmployeeModel>();
        //    var (sql, ps) = QueryBuilder.Build(tableName, filter);

        //    using (var conn = new SQLiteConnection($"Data Source={dataBaseFilePath}"))
        //    using (var cmd = new SQLiteCommand(sql, conn))
        //    {
        //        cmd.Parameters.AddRange(ps);
        //        conn.Open();
        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                employees.Add(_materializer.Map<EmployeeModel>(reader));
        //            }
        //        }
        //    }

        //    return employees;
        //}

    }
}
