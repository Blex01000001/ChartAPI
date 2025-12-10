using ChartAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using ChartAPI.DTOs;
using ChartAPI.Services.Queries;
using ChartAPI.Models;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.Assemblers.Charts;

namespace ChartAPI.Services.Chart
{
    public class AnnualSummaryService : IAnnualSummaryService
    {
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly  IManHourQueryService _manHourQuery;

        public AnnualSummaryService(
            IManHourQueryService _manHourQuery,
            IHubContext<NotifyHub> hubContext)
        {
            this._manHourQuery = _manHourQuery;
            this._hubContext = hubContext;
        }
        public AnnualSummaryDto GetChart(int year, string name, string id)
        {
            var qb = new QueryBuilder<ManHourModel>("ManHour")
                .Where(x => x.Year == year);
            if (!string.IsNullOrWhiteSpace(id))
                qb.Where(x => x.ID == id);
            if (!string.IsNullOrWhiteSpace(name))
                qb.Where(x => x.Name == name);

            List<ManHourModel> manhours = _manHourQuery.GetByQB(qb);
            return new AnnualSummaryAssembler<ManHourModel>(manhours)
                .SetName(name)
                .Assemble();


            //return new AnnualSummaryDto
            //{
            //    //monthlyChartDtos = new MonthlyChartBuilder<ManHourModel>(manhours)
            //    //    .Build(),
            //    monthlyChartDtos = manhours
            //        .GroupByProperty("Month")
            //        .OrderByDescending(x => x.Key)
            //        .Select(monthGroup => new MonthlyChartDto()
            //        {
            //            Month = (int)monthGroup.Key,
            //            PieChartDic = new Dictionary<string, PieChartDto>()
            //            {
            //                { "WorkNo", new PieChartBuilder<ManHourModel>(monthGroup.ToList(), "WorkNo", "Hours").Build()},
            //                { "CostCode", new PieChartBuilder<ManHourModel>(monthGroup.ToList(), "CostCode", "Hours").Build()}
            //            },
            //            StackCharts = new StackChartBuilder<ManHourModel>(monthGroup, "WorkNo", "Hours")
            //                .SetSeries(new StackSerie("Regular", "Regular", true, "Regular"))
            //                .SetSeries(new StackSerie("Overtime", "Overtime", true, "Overtime"))
            //                .Build()
            //        }).ToList(),

            //    stackChartDtos = new List<StackChartDto>(){ 
            //        new StackChartBuilder<ManHourModel>(manhours, "Month", "Hours")
            //        .SetName($"{name} 加班/請假年度統計")
            //        .SetSeries(new StackSerie("Overtime", "Overtime", true))
            //        .SetSeries(new StackSerie("Annual Paid", "CostCode", "003", "Leave"))
            //        .SetSeries(new StackSerie("Compensatory", "CostCode", "053", "Leave"))
            //        .SetSeries(new StackSerie("Common sick", "CostCode", "002", "Leave"))
            //        .SetSeries(new StackSerie("Personal", "CostCode", "001", "Leave"))
            //        .Build() 
            //    }
            //};
        }

    }
}
