using ChartAPI.DTOs;

namespace ChartAPI.Services.Chart
{
    public interface ICalendarSummaryService
    {
        Task<List<CalendarSummaryDto>> GetChart(string name, string id);
    }
}
