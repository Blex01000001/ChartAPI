using ChartAPI.DTOs;
using ChartAPI.Models;

namespace ChartAPI.Interfaces
{
    public interface IChartServices
    {
        List<YearCalendarDataDto> GetCalendarData(string name, string id);
        List<MonthlyChartData> GetMonthlyData(int year, string name, string id);
        StackChart<ManHourModel> GetStackChart(int year, string name, string id);
        void UpsertData(string name = null, string id = null);
    }
}
