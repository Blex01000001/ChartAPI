using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Models;
using ChartAPI.Models.Filters;

namespace ChartAPI.Services.Queries
{
    public interface IManHourQueryService
    {
        public List<ManHourModel> GetByFilter(IFilter filter);
    }
}
