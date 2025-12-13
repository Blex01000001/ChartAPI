using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.Models.Filters;
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
