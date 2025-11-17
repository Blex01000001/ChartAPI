using ChartAPI.DTOs;
using ChartAPI.Models;

namespace ChartAPI.ResponseDto
{
    public class MonthlyChartResponseDto
    {
        public List<MonthlyChartDto> monthlyChartDtos { get; set; } = new List<MonthlyChartDto>();
        public List<StackChartDto<ManHourModel>> stackChartDtos { get; set; } = new List<StackChartDto<ManHourModel>>();

    }
}
