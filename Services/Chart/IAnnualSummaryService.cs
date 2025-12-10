using ChartAPI.DTOs;

namespace ChartAPI.Services.Chart
{
    public interface IAnnualSummaryService
    {
        AnnualSummaryDto GetChart(int year, string name, string id);
    }
}
