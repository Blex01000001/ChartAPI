
namespace ChartAPI.DTOs.Charts.Stack
{
    public class StackChartDto
    {
        public string Name { get; private set; }
        public string[] AxisTitle { get; private set; }
        public List<StackSerie> Series { get; private set; }
        public StackChartDto(string name, string[] axisTitle, List<StackSerie> series)
        {
            Name = name;
            AxisTitle = axisTitle;
            Series = series;
        }
    }
}
