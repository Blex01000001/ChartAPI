using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.DTOs
{
    public class AnnualSummaryDto
    {
        public List<MonthlyChartDto> monthlyChartDtos { get; set; } = new List<MonthlyChartDto>();
        public List<StackChartDto> stackChartDtos { get; set; } = new List<StackChartDto>();
    }
}
