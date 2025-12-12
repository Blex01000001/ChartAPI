namespace ChartAPI.ChartBuilders
{
    public interface IChartBuilder<TDto>
    {
        TDto Build();
    }
}
