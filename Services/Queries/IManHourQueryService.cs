using ChartAPI.DataAccess.Interfaces;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.Domain.Entities;

namespace ChartAPI.Services.Queries
{
    public interface IManHourQueryService
    {
        //public List<ManHourModel> GetByFilter(IFilter filter);
        List<ManHour> GetByQB<T>(QueryBuilder<T> qb);
    }
}
