using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.ChartBuilders.Stack
{
    public interface IStackChartBuilder<T>
    {
        StackChartDto Build();
        StackChartBuilder<T> SetSeries(StackSerie series);
        StackChartBuilder<T> SetName(string chartName);
    }
}
