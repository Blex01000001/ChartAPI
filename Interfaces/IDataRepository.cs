using ChartAPI.Models;

namespace ChartAPI.Interfaces

{
    public interface IDataRepository
    {
        IEnumerable<TModel> GetData<TModel,TFilter>(TFilter filter, string tableName) 
            where TModel : new();
        void UpsertData(EmployeeFilter filter, string tableName);
    }
}
