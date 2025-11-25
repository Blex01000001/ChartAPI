namespace ChartAPI.Services.Upsert
{
    public interface IUpsertDataService
    {
        Task UpsertDataAsync(string name = null, string id = null);
    }
}
