using ChartAPI.Models.Filters;
using ChartAPI.Repositories.Query;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace ChartAPI.DataAccess.SQLite.QueryBuilders
{
    /// <summary>
    /// 對外使用的 QueryBuilder：負責組出完整 SQL + 參數
    /// </summary>
    public static class QueryBuilder
    {
        public static (string Sql, SQLiteParameter[] Params) Build(string tableName, IFilter filter)
        {
            StringBuilder sb = new StringBuilder();
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();

            sb.Append($"SELECT * FROM {tableName} WHERE 1=1");

            Dictionary<string, object> fields = filter.GetRawFields();
            SqlBuilderContext context = new SqlBuilderContext();

            foreach (var kv in fields)
            {
                string key = kv.Key;
                object value = kv.Value;

                if (value == null) continue;

                SqlBuildResult sqlResult = context.Build(key, value);

                if (!string.IsNullOrWhiteSpace(sqlResult.SqlFragment))
                {
                    sb.Append(sqlResult.SqlFragment);
                    if (sqlResult.Parameters != null && sqlResult.Parameters.Count > 0)
                        parameters.AddRange(sqlResult.Parameters);
                }
            }
            return (sb.ToString(), parameters.ToArray());
        }
    }
}
