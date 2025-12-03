using System.Data.SQLite;
using System.Linq.Expressions;

namespace ChartAPI.DataAccess.SQLite.QueryBuilders
{
    public sealed class ExpressionResult
    {
        public string SqlCondition { get; }
        public List<SQLiteParameter> Parameters { get; }

        public ExpressionResult(string sqlCondition, List<SQLiteParameter> parameters)
        {
            SqlCondition = sqlCondition;
            Parameters = parameters;
        }
    }

    // 將 LambdaExpression 轉成 SQL 片段的介面
    public interface IExpressionTranslator
    {
        ExpressionResult Translate(LambdaExpression expression, int paramStartIndex);
    }
}
