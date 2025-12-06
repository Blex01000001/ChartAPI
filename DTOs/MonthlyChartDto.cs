using Swashbuckle.AspNetCore.SwaggerGen;
using ChartAPI.Models;
using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.DTOs
{
    public class MonthlyChartDto
    {
        public int Month { get; set; }
        public Dictionary<string, PieChartDto> PieChartDic { get; set; }
        public StackChartDto StackCharts { get; set; }
    }
}
