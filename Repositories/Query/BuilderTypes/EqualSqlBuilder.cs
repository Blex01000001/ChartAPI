using System.Data.SQLite;

namespace ChartAPI.Repositories.Query.BuilderTypes
{
    /// <summary>
    /// 預設：一般等號比對 (=)
    /// （放在最後，前面 Builder 若都不能處理就會使用它）
    /// </summary>
    public class EqualSqlBuilder : ISqlBuilder
    {
        public SqlBuildResult Build(string key, object value)
        {
            var result = new SqlBuildResult();

            // 簡單處理空字串：直接略過，不產生條件
            if (value is string s && string.IsNullOrWhiteSpace(s))
                return result;

            string paramName = $"@{key}";

            result.SqlFragment = $" AND {key} = {paramName}";
            result.Parameters.Add(new SQLiteParameter(paramName, value ?? DBNull.Value));

            return result;
        }
    }
}
