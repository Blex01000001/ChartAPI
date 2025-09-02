using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChartAPI.Models
{
    public class MonthlyChartData
    {
        public int Month { get; set; } // 1~12
        public Dictionary<string, PieChartData> PieChartDic { get; set; }
        public StackChartData<ManHourModel> StackCharts { get; set; }
    }
}
