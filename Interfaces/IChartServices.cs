using ChartAPI.DTOs;
using ChartAPI.Models;

namespace ChartAPI.Interfaces
{
    public interface IChartServices
    {
        List<YearCalendarDto> GetCalendarData(string name, string id);
        List<MonthlyChartDto> GetMonthlyData(int year, string name, string id);
        StackChartDto<ManHourModel> GetStackChart(int year, string name, string id);
        DashboardResponseDto GetDashboardResponseDto(int year, string name, string id);
        void UpsertData(string name = null, string id = null);
    }
}
