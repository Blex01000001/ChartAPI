using ChartAPI.Models;

namespace ChartAPI.DTOs
{
    public class AnnualSummaryDto
    {
        public List<MonthlyChartDto> monthlyChartDtos { get; set; }
        public List<StackChartDto<ManHourModel>> stackChartDtos { get; set; } = new List<StackChartDto<ManHourModel>>();
    }
}
