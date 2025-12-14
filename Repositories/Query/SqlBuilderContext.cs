using ChartAPI.Enums;

namespace ChartAPI.Repositories.Query
{
    /// <summary>
    /// Builder 的管理器：負責根據 key/value 選擇適合的 ISqlBuilder
    /// </summary>
    //public class SqlBuilderContext
    //{
    //    public SqlBuildResult Build(string key, object value)
    //    {
    //        BuilderType builderEnum = GetBuilderType(key, value);
    //        Type builderType = Type.GetType($"ChartAPI.Repositories.Query.BuilderTypes.{builderEnum}");
    //        ISqlBuilder sqlBuilder = (ISqlBuilder)Activator.CreateInstance(builderType);
    //        return sqlBuilder.Build(key, value);
    //    }
    //    /// <summary>
    //    /// 判斷該用哪一個SQL Builder
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    private BuilderType GetBuilderType(string key, object value)
    //    {
    //        if (value is System.Collections.IEnumerable enumerable && value is not string)
    //        {
    //            return BuilderType.InSqlBuilder;
    //        }
    //        else if (key.EndsWith("Contains", StringComparison.OrdinalIgnoreCase) && value is string)
    //        {
    //            return BuilderType.LikeSqlBuilder;
    //        }
    //        else if (value is bool)
    //        {
    //            return BuilderType.BooleanSqlBuilder;
    //        }
    //        return BuilderType.EqualSqlBuilder;
    //    }
    //}
}
