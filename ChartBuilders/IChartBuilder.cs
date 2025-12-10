namespace ChartAPI.ChartBuilders
{
    public interface IChartBuilder<TChart>
    {
        TChart Build();
    }
}
