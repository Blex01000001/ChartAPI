using ChartAPI.Models;
using ChartAPI.Models.Filters;

namespace ChartAPI.DataAccess.Interfaces
{
    public interface IManHourRepository : IRepository<ManHourModel>
    {
        void UpdateToDataBase(List<ManHourModel> manHourModels);

    }
}
