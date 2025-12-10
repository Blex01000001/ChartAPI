using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.Models.Filters;

namespace ChartAPI.DataAccess.Interfaces
{
    public interface IRepository<TModel>
    {
        IEnumerable<TModel> GetByQBAsync<T>(QueryBuilder<T> qb);
        Task InsertAsync(IEnumerable<TModel> models);
        Task DeleteAsync(IEnumerable<TModel> models);
    }
}
