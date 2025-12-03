using ChartAPI.DataAccess.Interfaces;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.DataAccess.SQLite.Repositories;
using ChartAPI.Models;
using ChartAPI.Models.Filters;

namespace ChartAPI.Services.Queries
{
    public class ManHourQueryService : IManHourQueryService
    {
        private IManHourRepository _manhourRepo;

        public ManHourQueryService(IManHourRepository repo)
        {
            this._manhourRepo = repo;
        }

        List<ManHourModel> IManHourQueryService.GetByFilter(IFilter filter)
        {
            return _manhourRepo.GetByFilterAsync(filter).ToList();
        }

        List<ManHourModel> IManHourQueryService.GetByQB<T>(QueryBuilder<T> qb)
        {
            return _manhourRepo.GetByQBAsync<T>(qb).ToList();
        }
    }
}
