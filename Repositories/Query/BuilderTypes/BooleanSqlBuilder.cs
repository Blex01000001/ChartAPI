using System.Data.SQLite;

namespace ChartAPI.Repositories.Query.BuilderTypes
{
    /// <summary>
    /// bool → = 1 / 0
    /// </summary>
    //public class BooleanSqlBuilder : ISqlBuilder
    //{
    //    public SqlBuildResult Build(string key, object value)
    //    {
    //        var result = new SqlBuildResult();

    //        string paramName = $"@{key}";
    //        int intVal = (bool)value ? 1 : 0;

    //        result.SqlFragment = $" AND {key} = {paramName}";
    //        result.Parameters.Add(new SQLiteParameter(paramName, intVal));

    //        return result;
    //    }
    //}
}
