namespace ChartAPI.Repositories.Filters
{
    public interface IFilter
    {
        Dictionary<string, object> GetRawFields();
    }
}
