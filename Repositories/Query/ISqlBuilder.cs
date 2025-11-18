namespace ChartAPI.Repositories.Query
{
    /// <summary>
    /// 所有 SQL Builder 的共同介面
    /// </summary>
    public interface ISqlBuilder
    {
        bool CanBuild(string key, object value);
        SqlBuildResult Build(string key, object value);
    }
}
