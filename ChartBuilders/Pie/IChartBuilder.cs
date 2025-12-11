namespace ChartAPI.ChartBuilders.Pie
{
    public interface IChartBuilder<TChart>
    {
        TChart Build();
    }
}
