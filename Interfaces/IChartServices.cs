using ChartAPI.DTOs;
using ChartAPI.Models;

namespace ChartAPI.Interfaces
{
    public interface IChartServices
    {
        List<YearCalendarDataDto> GetCalendarData(string name, string id);
        List<MonthlyChartData> GetMonthlyData(string name, int year, string id);
    }
}
