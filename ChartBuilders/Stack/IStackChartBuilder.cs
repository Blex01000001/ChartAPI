using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.ChartBuilders.Stack
{
    public interface IStackChartBuilder<TModel> : IChartBuilder<StackChartDto>
    {
        IStackChartBuilder<TModel> SetSeries(StackSerie series);
        IStackChartBuilder<TModel> SetName(string chartName);
    }
}
