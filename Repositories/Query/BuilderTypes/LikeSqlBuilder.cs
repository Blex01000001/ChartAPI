using System.Data.SQLite;

namespace ChartAPI.Repositories.Query.BuilderTypes
{
    /// <summary>
    /// XxxContains → LIKE 條件
    /// </summary>
    //public class LikeSqlBuilder : ISqlBuilder
    //{
    //    private const string Suffix = "Contains";
    //    public SqlBuildResult Build(string key, object value)
    //    {
    //        var result = new SqlBuildResult();

    //        string column = key[..^Suffix.Length]; // 去掉 "Contains"
    //        string paramName = $"@{key}";

    //        result.SqlFragment = $" AND {column} LIKE {paramName}";
    //        result.Parameters.Add(new SQLiteParameter(paramName, $"%{(string)value}%"));

    //        return result;
    //    }
    //}
}
