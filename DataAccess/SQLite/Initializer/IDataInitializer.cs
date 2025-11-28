namespace ChartAPI.DataAccess.SQLite.Initializer
{
    public interface IDataInitializer
    {
        Task EnsureTablesCreatedAsync();
        Task EnsureIndexesCreatedAsync();
    }
}
