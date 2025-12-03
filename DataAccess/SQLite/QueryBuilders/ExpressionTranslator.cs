using System.Data.SQLite;
using System.Linq.Expressions;
using System.Text;

namespace ChartAPI.DataAccess.SQLite.QueryBuilders
{
    public sealed class SqlExpressionTranslator : IExpressionTranslator
    {
        private readonly Type _modelType;

        public SqlExpressionTranslator(Type modelType)
        {
            _modelType = modelType;
        }

        public ExpressionResult Translate(LambdaExpression expression, int paramStartIndex)
        {
            var parameters = new List<SQLiteParameter>();
            int index = paramStartIndex;

            string AddParameter(object? value)
            {
                string name = $"@p{index++}";
                parameters.Add(new SQLiteParameter(name, value ?? DBNull.Value));
                return name;
            }
            string Visit(Expression node)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Lambda:
                        return Visit(((LambdaExpression)node).Body);

                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        return Visit(((UnaryExpression)node).Operand);

                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.Add:
                    case ExpressionType.Subtract:
                    case ExpressionType.Multiply:
                    case ExpressionType.Divide:
                        {
                            var be = (BinaryExpression)node;
                            var leftSql = Visit(be.Left);
                            var rightSql = Visit(be.Right);
                            var op = GetSqlOperator(be.NodeType);
                            return $"({leftSql} {op} {rightSql})";
                        }

                    case ExpressionType.MemberAccess:
                        {
                            var me = (MemberExpression)node;

                            // x => x.Property → 欄位名稱
                            if (me.Expression is ParameterExpression)
                            {
                                return me.Member.Name;
                            }

                            // 其他 Member（閉包變數、常數）→ 當成值 Evaluate
                            var value = Evaluate(me);
                            return AddParameter(value);
                        }

                    case ExpressionType.Constant:
                        {
                            var ce = (ConstantExpression)node;
                            return AddParameter(ce.Value);
                        }

                    //case ExpressionType.Call:
                    //    {
                    //        var mc = (MethodCallExpression)node;
                    //        return VisitMethodCall(mc);
                    //    }

                    case ExpressionType.Not:
                        {
                            var unary = (UnaryExpression)node;
                            var inner = Visit(unary.Operand);
                            return $"(NOT {inner})";
                        }

                    default:
                        // 不支援的型別先 Evaluate 掉當常數
                        var v = Evaluate(node);
                        return AddParameter(v);
                }
            }

            //string VisitMethodCall(MethodCallExpression mc)
            //{
            //    // 支援 string.Contains / StartsWith / EndsWith
            //    if (mc.Object != null &&
            //        mc.Object.Type == typeof(string) &&
            //        mc.Arguments.Count == 1 &&
            //        mc.Arguments[0].Type == typeof(string))
            //    {
            //        var memberSql = Visit(mc.Object);
            //        var value = Evaluate(mc.Arguments[0])?.ToString() ?? string.Empty;

            //        return mc.Method.Name switch
            //        {
            //            nameof(string.Contains) => AddLike(memberSql, $"%{value}%"),
            //            nameof(string.StartsWith) => AddLike(memberSql, $"{value}%"),
            //            nameof(string.EndsWith) => AddLike(memberSql, $"%{value}"),
            //            _ => throw new NotSupportedException($"不支援的方法: {mc.Method.Name}")
            //        };
            //    }

            //    // 其他 MethodCall 先 Evaluate 成常數
            //    var result = Evaluate(mc);
            //    return AddParameter(result);
            //}

            //string AddLike(string column, string pattern)
            //{
            //    var paramName = $"p{index++}";
            //    parameters[paramName] = pattern;
            //    return $"{column} LIKE @{paramName}";
            //}

            //string AddParameter(object? value)
            //{
            //    var paramName = $"p{index++}";
            //    parameters[paramName] = value;
            //    return $"@{paramName}";
            //}

            object? Evaluate(Expression expr)
            {
                // 直接編譯成委派執行（簡版，之後可改成快取）
                var lambda = Expression.Lambda(expr);
                return lambda.Compile().DynamicInvoke();
            }

            string GetSqlOperator(ExpressionType nodeType) =>
                nodeType switch
                {
                    ExpressionType.AndAlso => "AND",
                    ExpressionType.OrElse => "OR",
                    ExpressionType.Equal => "=",
                    ExpressionType.NotEqual => "<>",
                    ExpressionType.GreaterThan => ">",
                    ExpressionType.GreaterThanOrEqual => ">=",
                    ExpressionType.LessThan => "<",
                    ExpressionType.LessThanOrEqual => "<=",
                    ExpressionType.Add => "+",
                    ExpressionType.Subtract => "-",
                    ExpressionType.Multiply => "*",
                    ExpressionType.Divide => "/",
                    _ => throw new NotSupportedException($"不支援的運算子: {nodeType}")
                };

            var sqlCondition = Visit(expression);

            return new ExpressionResult(sqlCondition, parameters);
        }
    }
}
