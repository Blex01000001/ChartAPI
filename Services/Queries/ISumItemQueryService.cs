using ChartAPI.Domain.Entities;
using SqlKata;

namespace ChartAPI.Services.Queries
{
    public interface ISumItemQueryService
    {
        IEnumerable<SumEntity> GetByQuery(Query query);
    }
}
