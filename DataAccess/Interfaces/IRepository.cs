using ChartAPI.Models.Filters;

namespace ChartAPI.DataAccess.Interfaces
{
    public interface IRepository<TModel>
    {
        IEnumerable<TModel> GetByFilterAsync(IFilter filter);

    }
}
