using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Hubs;
using ChartAPI.ResponseDto;
using Microsoft.AspNetCore.SignalR;
using ChartAPI.DTOs;
using ChartAPI.Services.Queries;
using ChartAPI.Models.Filters;
using ChartAPI.Models;
using ChartAPI.ChartBuilders;

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
            BaseFilter filter = new ManHourFilter();
            filter.Set("Year", year);
            if (!string.IsNullOrWhiteSpace(id))
                filter.Set("ID", id);
            if (!string.IsNullOrWhiteSpace(name))
                filter.Set("Name", name);
            List<ManHourModel> manhours = _manHourQuery.GetByFilter(filter);
            return new AnnualSummaryBuilder(manhours).Build();
        }

    }
}
