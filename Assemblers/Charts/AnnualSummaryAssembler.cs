using ChartAPI.ChartBuilders;
using ChartAPI.ChartBuilders.Stack;
using ChartAPI.ChartBuilders.Pie;
using ChartAPI.DTOs;
using ChartAPI.DTOs.Charts.Stack;
using ChartAPI.Models;
using System.Collections;
using System.Xml.Linq;
using ChartAPI.Extensions;
using ChartAPI.DTOs.Charts.Pie;

namespace ChartAPI.Assemblers.Charts
{
    public class AnnualSummaryAssembler<T>
    {
        private IEnumerable<T> _sourceDatas;
        private string _name = string.Empty;
        public AnnualSummaryAssembler(IEnumerable<T> datas)
        {
            _sourceDatas = datas;
        }

        public AnnualSummaryDto Assemble()
        {
            return new AnnualSummaryDto()
            {
                monthlyChartDtos = BuildMonthChart(),
                stackChartDtos = BuildAnnualStackChart()
            };
        }
        public AnnualSummaryAssembler<T> SetName(string name)
        {
            this._name = name;
            return this;
        }

        private List<StackChartDto> BuildAnnualStackChart()
        {
            return new List<StackChartDto>(){
                    new StackChartBuilder<T>(_sourceDatas, "Month", "Hours")
                    .SetName($"{_name} 加班/請假年度統計")
                    .SetSeries(new StackSerie("Overtime", "Overtime", true))
                    .SetSeries(new StackSerie("Annual Paid", "CostCode", "003", "Leave"))
                    .SetSeries(new StackSerie("Compensatory", "CostCode", "053", "Leave"))
                    .SetSeries(new StackSerie("Common sick", "CostCode", "002", "Leave"))
                    .SetSeries(new StackSerie("Personal", "CostCode", "001", "Leave"))
                    .Build()
                };
        }
        private List<MonthlyChartDto> BuildMonthChart()
        {
            return _sourceDatas
                    .GroupByProperty("Month")
                    .OrderByDescending(x => x.Key)
                    .Select(monthGroup => new MonthlyChartDto()
                    {
                        Month = (int)monthGroup.Key,
                        PieChartDic = new Dictionary<string, PieChartDto>()
                        {
                            { "WorkNo", new PieChartBuilder<T>(monthGroup.ToList(), "WorkNo", "Hours").Build()},
                            { "CostCode", new PieChartBuilder<T>(monthGroup.ToList(), "CostCode", "Hours").Build()}
                        },
                        StackCharts = new StackChartBuilder<T>(monthGroup, "WorkNo", "Hours")
                            .SetSeries(new StackSerie("Regular", "Regular", true, "Regular"))
                            .SetSeries(new StackSerie("Overtime", "Overtime", true, "Overtime"))
                            .Build()
                    }).ToList();
        }
    }
}
