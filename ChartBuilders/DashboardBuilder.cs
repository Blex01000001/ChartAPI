using ChartAPI.DTOs;
using ChartAPI.Models;

namespace ChartAPI.ChartBuilders
{
    public class DashboardBuilder
    {
        private DashboardResponseDto _responseDto;
        private IEnumerable<ManHourModel> _manHourList;
        public DashboardBuilder(IEnumerable<ManHourModel> manHourList)
        {
            _responseDto = new DashboardResponseDto();
            _manHourList = manHourList;
        }
        private StackChartDto<ManHourModel> CreateStackChartDto()
        {
            //新增每個Stack Series條件
            List<StackSeries> baseSeries = new List<StackSeries>()
            {
                new StackSeries("Overtime", "Overtime", true),
                new StackSeries("Annual Paid", "CostCode", "003", "Leave"),
                new StackSeries("Compensatory", "CostCode", "053", "Leave"),
                new StackSeries("Common sick", "CostCode", "002", "Leave"),
                new StackSeries("Personal", "CostCode", "001", "Leave")
            };
            //新增Stack Chart
            return new StackChartBuilder<ManHourModel>(_manHourList, "Month", baseSeries)
                .SetName("加班/請假年度統計")
                .Build();

        }
        private List<MonthlyChartDto> CreateMonthlyChartDto()
        {
            //新增Stack Series條件
            List<StackSeries> baseSeries = new List<StackSeries>()
            {
                new StackSeries("Regular", "Regular", true, "Regular"),
                new StackSeries("Overtime", "Overtime", true, "Overtime")
            };
            //依照MonthlyChartData形式分組
            return _manHourList
                .GroupBy(x => x.Month)
                .OrderByDescending(g => g.Key)
                .Select(monthGroup => new MonthlyChartDto()
                {
                    Month = monthGroup.Key,
                    PieChartDic = new Dictionary<string, PieChartDto>()
                    {
                        { "WorkNo", new PieChartBuilder<ManHourModel>(monthGroup.ToList(), "WorkNo").Build()},
                        { "CostCode", new PieChartBuilder<ManHourModel>(monthGroup.ToList(), "CostCode").Build()}
                    },
                    StackCharts = new StackChartBuilder<ManHourModel>(monthGroup.ToList(), "WorkNo", baseSeries)
                    .Build()
                }).ToList();
        }
        public DashboardResponseDto Build()
        {
            _responseDto.monthlyChartDtos = CreateMonthlyChartDto();
            _responseDto.stackChartDtos.Add(CreateStackChartDto());

            return _responseDto;
        }
    }
}
