using ChartAPI.Repositories.Filters;
using System.Data.SQLite;
using System.Text;

namespace ChartAPI.Repositories
{
    public static class QueryBuilder
    {
        public static (string Sql, SQLiteParameter[] Params) Build(string tableName, IFilter filter)
        {
            var sb = new StringBuilder();
            var parameters = new List<SQLiteParameter>();

            sb.Append($"SELECT * FROM {tableName} WHERE 1=1 ");

            var fields = filter.GetRawFields();

            foreach (var (key, value) in fields)
            {
                if (value == null)
                    continue;

                // ======================
                // 1) IN (List)
                // ======================
                if (value is System.Collections.IEnumerable list && !(value is string))
                {
                    var arr = list.Cast<object>().ToArray();
                    if (arr.Length == 0)
                        continue;

                    var inNames = new List<string>();

                    for (int i = 0; i < arr.Length; i++)
                    {
                        var paramName = $"@{key}_{i}";
                        inNames.Add(paramName);
                        parameters.Add(new SQLiteParameter(paramName, arr[i]));
                    }

                    sb.Append($" AND {key} IN ({string.Join(",", inNames)}) ");
                    continue;
                }

                // ======================
                // 2) Range (DateFrom / DateTo)
                // ======================
                if (key.EndsWith("From", StringComparison.OrdinalIgnoreCase))
                {
                    string col = key.Replace("From", "");
                    string paramName = $"@{key}";
                    sb.Append($" AND {col} >= {paramName} ");
                    parameters.Add(new SQLiteParameter(paramName, value));
                    continue;
                }

                if (key.EndsWith("To", StringComparison.OrdinalIgnoreCase))
                {
                    string col = key.Replace("To", "");
                    string paramName = $"@{key}";
                    sb.Append($" AND {col} <= {paramName} ");
                    parameters.Add(new SQLiteParameter(paramName, value));
                    continue;
                }

                // ======================
                // 3) LIKE (Contains 查詢)
                // ======================
                if (key.EndsWith("Contains", StringComparison.OrdinalIgnoreCase))
                {
                    string col = key.Replace("Contains", "");
                    string paramName = $"@{key}";
                    sb.Append($" AND {col} LIKE {paramName} ");
                    parameters.Add(new SQLiteParameter(paramName, $"%{value}%"));
                    continue;
                }

                // ======================
                // 4) Boolean → = 1 or 0
                // ======================
                if (value is bool b)
                {
                    string paramName = $"@{key}";
                    sb.Append($" AND {key} = {paramName} ");
                    parameters.Add(new SQLiteParameter(paramName, b ? 1 : 0));
                    continue;
                }

                // ======================
                // 5) 值型別 → 等號
                // ======================
                {
                    string paramName = $"@{key}";
                    sb.Append($" AND {key} = {paramName} ");
                    parameters.Add(new SQLiteParameter(paramName, value));
                }
            }

            return (sb.ToString(), parameters.ToArray());
        }



        // ----------------------------------------
        // 專為 SumFilter 設計（含 JOIN / GROUP）
        // ----------------------------------------
        public static (string Sql, SQLiteParameter[] Params) BuildSumQuery(string tableName, SumFilter filter)
        {
            var sb = new StringBuilder();
            var parameters = new List<SQLiteParameter>();

            sb.AppendLine("SELECT ");

            // SUM 查詢固定輸出
            sb.AppendLine("   mh.ID, mh.Name, SUM(mh.Hours) AS TotalHours ");

            sb.AppendLine($"FROM {tableName} AS mh");

            // =========== JOIN EmpInfo ===========
            if (filter.JoinEmpInfo)
            {
                sb.AppendLine(@"LEFT JOIN EmpInfo9933 AS emp
                               ON mh.ID = emp.employee_id");
            }

            sb.AppendLine("WHERE 1=1");

            var fields = filter.GetRawFields();

            foreach (var (key, value) in fields)
            {
                if (value == null)
                    continue;

                if (key == nameof(SumFilter.JoinEmpInfo))
                    continue;

                if (key == nameof(SumFilter.GroupBy))
                    continue;

                if (key == nameof(SumFilter.OrderBy))
                    continue;

                // Special cases
                if (key == nameof(SumFilter.Group2Contains))
                {
                    sb.AppendLine(" AND emp.Group2 LIKE @g2 ");
                    parameters.Add(new SQLiteParameter("@g2", $"%{value}%"));
                    continue;
                }

                if (key == nameof(SumFilter.OnlyOvertime) && value is bool b)
                {
                    if (b)
                        sb.AppendLine(" AND mh.Overtime = 1 ");
                    continue;
                }

                if (key == nameof(SumFilter.CostCodes))
                {
                    var codes = (value as System.Collections.IEnumerable)?
                                    .Cast<object>()
                                    .Select(x => x.ToString())
                                    .ToArray();

                    if (codes == null || codes.Length == 0)
                        continue;

                    var names = new List<string>();

                    for (int i = 0; i < codes.Length; i++)
                    {
                        var p = $"@cc{i}";
                        names.Add(p);
                        parameters.Add(new SQLiteParameter(p, codes[i]));
                    }

                    sb.AppendLine($" AND mh.CostCode IN ({string.Join(",", names)}) ");
                    continue;
                }

                // Default: Equal
                var param = $"@{key}";
                sb.AppendLine($" AND mh.{key} = {param} ");
                parameters.Add(new SQLiteParameter(param, value));
            }

            // =========== GROUP BY ===========
            if (filter.GroupBy.Any())
            {
                sb.AppendLine("GROUP BY " +
                    string.Join(",", filter.GroupBy.Select(x => $"mh.{x}")));
            }
            else
            {
                sb.AppendLine("GROUP BY mh.ID, mh.Name");
            }

            // =========== ORDER BY ===========
            if (filter.OrderBy.Any())
            {
                sb.AppendLine("ORDER BY " +
                    string.Join(",", filter.OrderBy.Select(x => x)));
            }
            else
            {
                sb.AppendLine("ORDER BY TotalHours DESC");
            }

            return (sb.ToString(), parameters.ToArray());
        }
    }
}
