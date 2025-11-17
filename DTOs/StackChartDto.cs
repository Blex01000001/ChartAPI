using ChartAPI.Models;

namespace ChartAPI.DTOs
{
    public class StackChartDto<T>
    {
        public string Name { get; private set; }
        public string[] AxisTitle { get; private set; }
        public List<StackSerie> Series { get; private set; }
        public StackChartDto(string name, string[] axisTitle, List<StackSerie> series)
        {
            this.Name = name;
            this.AxisTitle = axisTitle;
            this.Series = series;
        }
    }
}
