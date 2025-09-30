using ChartAPI.DTOs;
using ChartAPI.Models;

namespace ChartAPI.ChartBuilders
{
    public class DeptDashboardBuilder
    {
        private DeptChartDto _responseDto;
        private IEnumerable<ManHourModel> _manHourList;
        public DeptDashboardBuilder(IEnumerable<ManHourModel> manHourList)
        {
            _responseDto = new DeptChartDto();
            _manHourList = manHourList;
        }
        private StackChartDto<ManHourModel> CreateStackChartDto()
        {
            //新增每個Stack Series條件
            List<StackSeries> baseSeries = new List<StackSeries>()
            {
                //new StackSeries("Overtime", "Overtime", true),
                new StackSeries("Annual Paid", "CostCode", "003", "Leave"),
                //new StackSeries("Compensatory", "CostCode", "053", "Leave"),
                //new StackSeries("Common sick", "CostCode", "002", "Leave"),
                //new StackSeries("Personal", "CostCode", "001", "Leave")
            };
            //新增Stack Chart
            return new StackChartBuilder<ManHourModel>(_manHourList, "Name", baseSeries)
                .SetName("特休王年度統計")
                .Build();
        }

        public DeptChartDto Build()
        {
            _responseDto.stackChartDtos.Add(CreateStackChartDto());

            return _responseDto;
        }
    }
}
