using ChartAPI.Models;

namespace ChartAPI.Interfaces

{
    public interface IDataRepository
    {
        IEnumerable<ManHourModel> GetManHourData(ManHourFilter filter);
    }
}
