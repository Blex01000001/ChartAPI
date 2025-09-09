using ChartAPI.Models;

namespace ChartAPI.DTOs
{
    public class DashboardResponseDto
    {
        List<MonthlyChartDto> monthlyChartDtos { get; set; }
        List<StackChartDto<ManHourModel>> stackChartDtos { get; set; }
    }
}
