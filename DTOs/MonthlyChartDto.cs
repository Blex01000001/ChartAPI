using ChartAPI.DTOs.Charts.Stack;
using ChartAPI.DTOs.Charts.Pie;

namespace ChartAPI.DTOs
{
    public class MonthlyChartDto
    {
        public int Month { get; set; }
        public Dictionary<string, PieChartDto> PieChartDic { get; set; }
        public StackChartDto StackCharts { get; set; }
    }
}
