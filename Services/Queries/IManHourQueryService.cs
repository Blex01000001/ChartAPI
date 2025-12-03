using ChartAPI.DataAccess.Interfaces;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.Models;
using ChartAPI.Models.Filters;

namespace ChartAPI.Services.Queries
{
    public interface IManHourQueryService
    {
        public List<ManHourModel> GetByFilter(IFilter filter);
        List<ManHourModel> GetByQB<T>(QueryBuilder<T> qb);
    }
}
