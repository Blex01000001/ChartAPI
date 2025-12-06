using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.ChartBuilders.Stack
{
    public interface IStackChartBuilder<T>
    {
        StackChartDto Build();
    }
}
