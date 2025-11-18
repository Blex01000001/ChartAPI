using ChartAPI.Repositories.Filters;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace ChartAPI.Repositories.Query
{
    /// <summary>
    /// 對外使用的 QueryBuilder：負責組出完整 SQL + 參數
    /// </summary>
    public static class QueryBuilder
    {
        public static (string Sql, SQLiteParameter[] Params) Build(string tableName, IFilter filter)
        {
            var sb = new StringBuilder();
            var parameters = new List<SQLiteParameter>();

            sb.Append($"SELECT * FROM {tableName} WHERE 1=1");

            var fields = filter.GetRawFields();
            var context = new SqlBuilderContext();

            foreach (var kv in fields)
            {
                string key = kv.Key;
                object value = kv.Value;

                if (value == null)
                    continue;

                var part = context.Build(key, value);

                if (!string.IsNullOrWhiteSpace(part.SqlFragment))
                {
                    sb.Append(part.SqlFragment);
                    if (part.Parameters != null && part.Parameters.Count > 0)
                        parameters.AddRange(part.Parameters);
                }
            }

            return (sb.ToString(), parameters.ToArray());
        }
    }
}
