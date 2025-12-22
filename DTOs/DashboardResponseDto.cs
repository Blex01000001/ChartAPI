using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.DTOs
{
    public class DashboardResponseDto
    {
        public List<MonthlyChartDto> monthlyChartDtos { get; set; }
        public List<StackChartDto> stackChartDtos { get; set; } = new List<StackChartDto>();
    }
}
