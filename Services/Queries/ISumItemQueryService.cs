using ChartAPI.Models;
using SqlKata;

namespace ChartAPI.Services.Queries
{
    public interface ISumItemQueryService
    {
        IEnumerable<SumItem> GetByQuery(Query query);
    }
}
