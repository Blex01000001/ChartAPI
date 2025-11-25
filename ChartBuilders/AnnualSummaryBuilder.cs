using ChartAPI.DTOs;
using ChartAPI.Models;
using ChartAPI.ResponseDto;

namespace ChartAPI.ChartBuilders
{
    public class AnnualSummaryBuilder
    {
        private AnnualSummaryDto _responseDto;
        private IEnumerable<ManHourModel> _manHourList;
        public AnnualSummaryBuilder(IEnumerable<ManHourModel> manHourList)
        {
            _responseDto = new AnnualSummaryDto();
            _manHourList = manHourList;
        }
        private StackChartDto<ManHourModel> CreateStackChartDto()
        {
            //新增每個Stack Series條件
            List<StackSerie> Series = new List<StackSerie>()
            {
                new StackSerie("Overtime", "Overtime", true),
                new StackSerie("Annual Paid", "CostCode", "003", "Leave"),
                new StackSerie("Compensatory", "CostCode", "053", "Leave"),
                new StackSerie("Common sick", "CostCode", "002", "Leave"),
                new StackSerie("Personal", "CostCode", "001", "Leave")
            };
            //新增Stack Chart
            return new StackChartBuilder<ManHourModel>(_manHourList, "Month", "Hours", Series)
                .SetName("加班/請假年度統計")
                .Build();

        }
        private List<MonthlyChartDto> CreateMonthlyChartDto()
        {
            //新增Stack Series條件
            List<StackSerie> baseSeries = new List<StackSerie>()
            {
                new StackSerie("Regular", "Regular", true, "Regular"),
                new StackSerie("Overtime", "Overtime", true, "Overtime")
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
                    StackCharts = new StackChartBuilder<ManHourModel>(monthGroup.ToList(), "WorkNo", "Hours", baseSeries)
                    .Build()
                }).ToList();
        }
        public AnnualSummaryDto Build()
        {
            _responseDto.monthlyChartDtos = CreateMonthlyChartDto();
            _responseDto.stackChartDtos.Add(CreateStackChartDto());

            return _responseDto;
        }
    }
}
