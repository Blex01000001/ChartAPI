using ChartAPI.Models;

namespace ChartAPI.DTOs
{
    public class DeptChartDto
    {
        public List<StackChartDto<SumModel>> stackChartDtos { get; set; } = new List<StackChartDto<SumModel>>();
    }
}
