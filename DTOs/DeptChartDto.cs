using ChartAPI.Models;

namespace ChartAPI.DTOs
{
    public class DeptChartDto
    {
        public List<StackChartDto<ManHourModel>> stackChartDtos { get; set; } = new List<StackChartDto<ManHourModel>>();
    }
}
