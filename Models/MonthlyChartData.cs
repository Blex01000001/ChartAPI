using Swashbuckle.AspNetCore.SwaggerGen;
using ChartAPI.DTOs;

namespace ChartAPI.Models
{
    public class MonthlyChartData
    {
        public int Month { get; set; }
        public Dictionary<string, PieChartDto> PieChartDic { get; set; }
        public StackChartDto<ManHourModel> StackCharts { get; set; }
    }
}
