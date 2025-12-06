using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Hubs;
using ChartAPI.ResponseDto;
using Microsoft.AspNetCore.SignalR;
using ChartAPI.DTOs;
using ChartAPI.Services.Queries;
using ChartAPI.Models.Filters;
using ChartAPI.Models;
using ChartAPI.ChartBuilders;
using Microsoft.AspNetCore.Http.Extensions;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.ChartBuilders.AnnualSummary;
using ChartAPI.DTOs.Charts.Stack;
using ChartAPI.ChartBuilders.MonthlyChart;
using ChartAPI.ChartBuilders.Stack;

namespace ChartAPI.Services.Chart
{
    public class AnnualSummaryService : IAnnualSummaryService
    {
        private readonly IHubContext<NotifyHub> _hubContext;
        private IManHourQueryService _manHourQuery;

        public AnnualSummaryService(IManHourQueryService _manHourQuery, IHubContext<NotifyHub> hubContext)
        {
            this._manHourQuery = _manHourQuery;
            this._hubContext = hubContext;
        }
        public AnnualSummaryDto GetAnnualSummary(int year, string name, string id)
        {
            var qb = new QueryBuilder<ManHourModel>("ManHour")
                .Where(x => x.Year == year);
            if (!string.IsNullOrWhiteSpace(id))
                qb.Where(x => x.ID == id);
            if (!string.IsNullOrWhiteSpace(name))
                qb.Where(x => x.Name == name);

            List<ManHourModel> manhours = _manHourQuery.GetByQB(qb);
            //return new AnnualSummaryBuilder(manhours).Build();

            return new AnnualSummaryDto
            {
                monthlyChartDtos = new MonthlyChartBuilder<ManHourModel>(manhours)
                    .Build(),
                stackChartDtos = new List<StackChartDto>(){ 
                    new StackChartBuilder<ManHourModel>(manhours, "Month", "Hours")
                    .SetName($"{name} 加班/請假年度統計")
                    .SetSeries(new List<StackSerie>
                    {
                        new StackSerie("Overtime", "Overtime", true),
                        new StackSerie("Annual Paid", "CostCode", "003", "Leave"),
                        new StackSerie("Compensatory", "CostCode", "053", "Leave"),
                        new StackSerie("Common sick", "CostCode", "002", "Leave"),
                        new StackSerie("Personal", "CostCode", "001", "Leave")
                    })
                    .Build() 
                }
            };



            //BaseFilter filter = new ManHourFilter();
            //filter.Set("Year", year);
            //if (!string.IsNullOrWhiteSpace(id))
            //    filter.Set("ID", id);
            //if (!string.IsNullOrWhiteSpace(name))
            //    filter.Set("Name", name);
            //List<ManHourModel> manhours = _manHourQuery.GetByFilter(filter);
            //return new AnnualSummaryBuilder(manhours).Build();
        }

    }
}
