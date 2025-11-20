using ChartAPI.Models;
using ChartAPI.Models.Filters;

namespace ChartAPI.Interfaces

{
    public interface IDataRepository
    {
        IEnumerable<TModel> GetData<TModel>(IFilter filter, string tableName) where TModel : new();
        Task UpsertData(IFilter filter, string tableName);
        SumModel GetSumData(int year, string group2Filter, string reportTitle, bool isOvertime = false, List<string> costCodes = null);

    }
}
