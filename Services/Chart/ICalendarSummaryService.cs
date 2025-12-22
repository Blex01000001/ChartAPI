using ChartAPI.DTOs;

namespace ChartAPI.Services.Chart
{
    public interface ICalendarSummaryService
    {
        Task<Dictionary<string, List<CalendarSummaryDto>>> GetChart(string name, string id);
    }
}
