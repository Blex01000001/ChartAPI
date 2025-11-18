using System.Data.SQLite;

namespace ChartAPI.Repositories.Query
{
    /// <summary>
    /// List / Array → IN (...)
    /// </summary>
    public class InSqlBuilder : ISqlBuilder
    {
        public bool CanBuild(string key, object value)
        {
            // List / Array / IEnumerable，但排除 string
            return value is System.Collections.IEnumerable enumerable && value is not string;
        }

        public SqlBuildResult Build(string key, object value)
        {
            var result = new SqlBuildResult();

            var enumerable = (value as System.Collections.IEnumerable)!;
            var items = enumerable.Cast<object>().ToArray();
            if (items.Length == 0)
                return result; // 不產生任何條件

            var paramNames = new List<string>();

            for (int i = 0; i < items.Length; i++)
            {
                var pName = $"@{key}_{i}";
                paramNames.Add(pName);
                result.Parameters.Add(new SQLiteParameter(pName, items[i] ?? DBNull.Value));
            }

            result.SqlFragment = $" AND {key} IN ({string.Join(",", paramNames)})";
            return result;
        }
    }
}
