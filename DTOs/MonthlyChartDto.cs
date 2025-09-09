using Swashbuckle.AspNetCore.SwaggerGen;
using ChartAPI.Models;

namespace ChartAPI.DTOs
{
    public class MonthlyChartDto
    {
        public int Month { get; set; }
        public Dictionary<string, PieChartDto> PieChartDic { get; set; }
        public StackChartDto<ManHourModel> StackCharts { get; set; }
    }
}
