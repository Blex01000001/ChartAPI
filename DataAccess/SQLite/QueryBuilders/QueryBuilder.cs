using ChartAPI.DataAccess.SQLite.QueryBuilders.Components;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Text;

namespace ChartAPI.DataAccess.SQLite.QueryBuilders
{
    /// <summary>
    /// 對外使用的 QueryBuilder：負責組出完整 SQL + 參數
    /// </summary>
    public sealed class QueryBuilder<TModel>
    {
        private readonly string _tableName;
        private string? _selectSql;

        private readonly WhereBuilder _whereBuilder = new();
        private readonly IExpressionTranslator _translator;

        public QueryBuilder(string tableName)
        {
            _tableName = tableName;
            _translator = new SqlExpressionTranslator(typeof(TModel));
            _selectSql = $"SELECT * FROM {_tableName}";
        }

        /// <summary>
        /// 覆寫 SELECT 子句，預設為ALL
        /// </summary>
        public QueryBuilder<TModel> Select(string selectSql)
        {
            _selectSql = selectSql;
            return this;
        }

        /// <summary>
        /// 使用 Expression 方式新增一個 WHERE 條件。
        /// 例如：qb.Where(x => x.Year > 2025 && x.EmployeeNo.StartsWith("A"));
        /// </summary>
        public QueryBuilder<TModel> Where(Expression<Func<TModel, bool>> predicate)
        {
            var fragment = _translator.Translate(predicate, _whereBuilder.NextParameterIndex);
            _whereBuilder.Add(fragment);
            return this;
        }

        /// <summary>
        /// 建立最終 SQL 與參數字典
        /// </summary>
        public (string Sql, SQLiteParameter[] Params) Build()
        {
            var sb = new StringBuilder();

            sb.Append(_selectSql);
            //sb.Append(" FROM ");
            //sb.Append(_tableName);

            var whereSql = _whereBuilder.Build();
            if (!string.IsNullOrEmpty(whereSql))
            {
                sb.Append(' ');
                sb.Append(whereSql);
            }

            return (sb.ToString(), _whereBuilder.Parameters);
        }
        //public static class QueryBuilder_Old
        //{
        //    public static (string Sql, SQLiteParameter[] Params) Build(string tableName, IFilter filter)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        List<SQLiteParameter> parameters = new List<SQLiteParameter>();

        //        sb.Append($"SELECT * FROM {tableName} WHERE 1=1");

        //        Dictionary<string, object> fields = filter.GetRawFields();
        //        SqlBuilderContext context = new SqlBuilderContext();

        //        foreach (var kv in fields)
        //        {
        //            string key = kv.Key;
        //            object value = kv.Value;

        //            if (value == null) continue;

        //            SqlBuildResult sqlResult = context.Build(key, value);

        //            if (!string.IsNullOrWhiteSpace(sqlResult.SqlFragment))
        //            {
        //                sb.Append(sqlResult.SqlFragment);
        //                if (sqlResult.Parameters != null && sqlResult.Parameters.Count > 0)
        //                    parameters.AddRange(sqlResult.Parameters);
        //            }
        //        }
        //        return (sb.ToString(), parameters.ToArray());
        //    }
        //}
    }
}
