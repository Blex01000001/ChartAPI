using ChartAPI.Assemblers.Charts;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.DTOs;
using ChartAPI.Hubs;
using ChartAPI.Models;
using ChartAPI.Services.Queries;
using Microsoft.AspNetCore.SignalR;

namespace ChartAPI.Services.Chart
{
    public class CalendarSummaryService : ICalendarSummaryService
    {
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly IManHourQueryService _manHourQuery;
        public CalendarSummaryService(
            IManHourQueryService _manHourQuery,
            IHubContext<NotifyHub> hubContext)
        {
            this._manHourQuery = _manHourQuery;
            this._hubContext = hubContext;
        }
        public List<CalendarSummaryDto> GetChart(string name, string id)
        {
            var qb = new QueryBuilder<ManHourModel>("ManHour")
                .Where(x => x.Name == name)
                .Where(x => x.CostCode == "003");
            if (!string.IsNullOrWhiteSpace(id))
                qb.Where(x => x.ID == id);

            List<ManHourModel> manhours = _manHourQuery.GetByQB(qb);
            return new CalendarSummaryAssembler<ManHourModel>(manhours)
                .Assemble();
        }
    }
}
