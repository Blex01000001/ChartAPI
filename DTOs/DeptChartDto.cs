using ChartAPI.DTOs.Charts.Stack;
using ChartAPI.Models;

namespace ChartAPI.DTOs
{
    public class DeptChartDto
    {
        public List<StackChartDto> stackChartDtos { get; set; } = new List<StackChartDto>();
    }
}
