using ChartAPI.DTOs;

namespace ChartAPI.ChartBuilders.MonthlyChart
{
    public interface IMonthlyChartBuilder<T>
    {
        List<MonthlyChartDto> Build();
    }
}
