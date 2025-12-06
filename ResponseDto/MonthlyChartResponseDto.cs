using ChartAPI.DTOs;
using ChartAPI.DTOs.Charts.Stack;
using ChartAPI.Models;

namespace ChartAPI.ResponseDto
{
    public class MonthlyChartResponseDto
    {
        public List<MonthlyChartDto> monthlyChartDtos { get; set; } = new List<MonthlyChartDto>();
        public List<StackChartDto> stackChartDtos { get; set; } = new List<StackChartDto>();

    }
}
