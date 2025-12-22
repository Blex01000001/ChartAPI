using ChartAPI.DataAccess.SQLite.Utilities;
using ChartAPI.Domain.Entities;
using ChartAPI.Extensions;
using SqlKata;
using SqlKata.Compilers;
using System.Data.SQLite;
using System.Diagnostics;

namespace ChartAPI.Services.Queries
{
    public class SumItemQueryService : ISumItemQueryService
    {
        protected readonly Materializer _materializer = new Materializer();
        protected readonly string _tableName;
        protected readonly string _dBFileName;
        protected readonly string _dataBaseDir;
        protected readonly string _dataBaseFilePath;

        public SumItemQueryService(IConfiguration config)
        {
            //this._tableName = tableName;
            this._dBFileName = "ManHourData.db";
            this._dataBaseDir = config.GetConnectionString("DataBaseDir")
                ?? throw new InvalidOperationException("Connection string 'DataBaseDir' is missing.");
            this._dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);
        }
        public IEnumerable<SumEntity> GetByQuery(Query query)
        {
            List<SumEntity> result = new List<SumEntity>();
            Stopwatch ExecuteReaderTime = new Stopwatch();
            Stopwatch AutoMapReaderTime = new Stopwatch();

            SqlResult sqlResult = new SqliteCompiler().Compile(query);

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = sqlResult.Sql;
                cmd.Parameters.AddRange(SqlKataSqliteHelper.ToSqliteParameters(sqlResult));
                ExecuteReaderTime.Start();

                using (var reader = cmd.ExecuteReader())
                {
                    ExecuteReaderTime.Stop();
                    AutoMapReaderTime.Start();
                    while (reader.Read())
                    {
                        result.Add(_materializer.Map<SumEntity>(reader));
                    }
                    AutoMapReaderTime.Stop();
                }
            }
            ConsoleExtensions.WriteLineWithTime($"Query Count: {result.Count}, SQL Execute {ExecuteReaderTime.ElapsedMilliseconds} ms, Materializer Elapsed {AutoMapReaderTime.ElapsedMilliseconds} ms");

            return result;
        }
        protected SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection($"Data Source={_dataBaseFilePath}");
        }

    }
}
