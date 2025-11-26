using ChartAPI.DataAccess.Interfaces;
using ChartAPI.DataAccess.SQLite.Utilities;
using ChartAPI.Models.Filters;
using System.Data.SQLite;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.Extensions;
using System.Diagnostics;

namespace ChartAPI.DataAccess.SQLite.Repositories
{
    public abstract class BaseRepository<TModel> : IRepository<TModel> where TModel : new()
    {
        protected readonly Materializer _materializer = new Materializer();
        protected readonly string _tableName;
        protected readonly string _dBFileName;
        protected readonly string _dataBaseDir;
        protected readonly string _dataBaseFilePath;

        protected BaseRepository(IConfiguration config, string tableName, string dbFileName)
        {
            this._tableName = tableName;
            this._dBFileName = dbFileName;
            this._dataBaseDir = config.GetConnectionString("DataBaseDir")
                ?? throw new InvalidOperationException("Connection string 'DataBaseDir' is missing.");
            this._dataBaseFilePath = Path.Combine(_dataBaseDir, _dBFileName);
        }

        /// <summary>
        /// IRepository 的共用實作，實際邏輯丟給 Query
        /// 子類別可以 override Query 來客製化
        /// </summary>
        IEnumerable<TModel> IRepository<TModel>.GetByFilterAsync(IFilter filter)
        {
            return Query(filter);
        }

        /// <summary>
        /// 預設查詢邏輯：給一般 Repository 用
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TModel> Query(IFilter filter)
        {
            List<TModel> result = new List<TModel>();
            Stopwatch ExecuteReaderTime = new Stopwatch();
            Stopwatch AutoMapReaderTime = new Stopwatch();

            var (sql, ps) = QueryBuilder.Build(_tableName, filter);

            using (var conn = CreateConnection())
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
                        result.Add(_materializer.Map<TModel>(reader));
                    }
                    AutoMapReaderTime.Stop();
                }
            }
            ConsoleExtensions.WriteLineWithTime($"Query Count: {result.Count}, SQL Execute {ExecuteReaderTime.ElapsedMilliseconds} ms, Materializer Elapsed {AutoMapReaderTime.ElapsedMilliseconds} ms");
            return result;
        }

        /// <summary>
        /// 提供給子類別使用的共用 Connection 產生器
        /// </summary>
        /// <returns></returns>
        protected SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection($"Data Source={_dataBaseFilePath}");
        }
    }
}
