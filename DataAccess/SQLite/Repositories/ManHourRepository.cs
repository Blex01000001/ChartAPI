using System.Data.SQLite;
using System.Diagnostics;
using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Extensions;
using ChartAPI.Models;
using ChartAPI.Models.Filters;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.DataAccess.SQLite.Utilities;

namespace ChartAPI.DataAccess.SQLite.Repositories
{
    public class ManHourRepository : IManHourRepository
    {
        private readonly Materializer _materializer = new Materializer();
        private readonly string _dataBaseDir;
        private string dataBaseFilePath;
        private string _dBFileName = "ManHourData.db";

        public ManHourRepository(IConfiguration config)
        {
            _dataBaseDir = config.GetConnectionString("DataBaseDir");
            dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);
        }

        IEnumerable<ManHourModel> IRepository<ManHourModel>.GetByFilterAsync(IFilter filter)
        {
            string tableName = "ManHour";
            List<ManHourModel> result = new List<ManHourModel>();
            Stopwatch ExecuteReaderTime = new Stopwatch();
            Stopwatch AutoMapReaderTime = new Stopwatch();

            var (sql, ps) = QueryBuilder.Build(tableName, filter);

            using (var conn = new SQLiteConnection($"Data Source={dataBaseFilePath}"))
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddRange(ps);
                conn.Open();
                ExecuteReaderTime.Start();
                using (var reader = cmd.ExecuteReader())
                {
                    ExecuteReaderTime.Stop();
                    AutoMapReaderTime.Start();
                    while (reader.Read())
                    {
                        result.Add(_materializer.Map<ManHourModel>(reader));
                    }
                    AutoMapReaderTime.Stop();
                }
            }
            ConsoleExtensions.WriteLineWithTime($"Query Count: {result.Count}, SQL Execute {ExecuteReaderTime.ElapsedMilliseconds} ms, Materializer Elapsed {AutoMapReaderTime.ElapsedMilliseconds} ms");
            return result;
        }


        public Task UpsertDataAsync(IFilter filter)
        {
            throw new NotImplementedException();
        }

    }
}
