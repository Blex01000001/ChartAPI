using ChartAPI.Models;

namespace ChartAPI.Interfaces

{
    public interface IDataRepository
    {
        IEnumerable<TModel> GetData<TModel,TFilter>(TFilter filter, string tableName) where TModel : new();
        Task UpsertData(EmployeeFilter filter, string tableName);
        SumModel GetSumData(int year, string group2Filter, string reportTitle, bool isOvertime = false, List<string> costCodes = null);

    }
}
