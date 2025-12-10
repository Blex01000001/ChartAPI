using ChartAPI.DTOs;

namespace ChartAPI.Services.Chart
{
    public interface ICalendarSummaryService
    {
        List<CalendarSummaryDto> GetChart(string name, string id);
    }
}
