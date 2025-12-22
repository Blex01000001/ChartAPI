using SqlKata;

namespace ChartAPI.DataAccess.Interfaces
{
    public interface IRepository<TModel>
    {
        IEnumerable<TModel> GetByQuery(Query query);
        Task InsertAsync(IEnumerable<TModel> models);
        Task DeleteAsync(IEnumerable<TModel> models);
    }
}
