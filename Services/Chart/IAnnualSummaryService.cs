using ChartAPI.ResponseDto;
using ChartAPI.DTOs;

namespace ChartAPI.Services.Chart
{
    public interface IAnnualSummaryService
    {
        AnnualSummaryDto GetAnnualSummary(int year, string name, string id);
    }
}
