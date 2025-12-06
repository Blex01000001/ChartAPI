using ChartAPI.ChartBuilders.Stack;
using ChartAPI.DTOs;
using ChartAPI.DTOs.Charts.Stack;
using ChartAPI.Extensions;
using ChartAPI.Models;

namespace ChartAPI.ChartBuilders.MonthlyChart
{
    public class MonthlyChartBuilder<T> : IMonthlyChartBuilder<T>
    {
        private readonly IEnumerable<T> _sourceData;
        public MonthlyChartBuilder(IEnumerable<T> sourceData)
        {
            _sourceData = sourceData;
        }

        public List<MonthlyChartDto> Build()
        {
            //新增Stack Series條件
            List<StackSerie> baseSeries = new List<StackSerie>()
            {
                new StackSerie("Regular", "Regular", true, "Regular"),
                new StackSerie("Overtime", "Overtime", true, "Overtime")
            };
            //依照MonthlyChartData形式分組
            return _sourceData
                .GroupByProperty("Month")
                .OrderByDescending(g => g.Key)
                .Select(monthGroup => new MonthlyChartDto()
                {
                    Month = (int)monthGroup.Key,
                    PieChartDic = new Dictionary<string, PieChartDto>()
                    {
                        { "WorkNo", new PieChartBuilder<T>(monthGroup.ToList(), "WorkNo").Build()},
                        { "CostCode", new PieChartBuilder<T>(monthGroup.ToList(), "CostCode").Build()}
                    },
                    StackCharts = new StackChartBuilder<T>(monthGroup.ToList(), "WorkNo", "Hours")
                    .SetSeries(baseSeries)
                    .Build()
                }).ToList();
        }
    }
}
