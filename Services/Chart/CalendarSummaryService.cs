using ChartAPI.Assemblers.Charts;
using ChartAPI.DataAccess.Interfaces;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.Domain.Entities;
using ChartAPI.DTOs;
using ChartAPI.Hubs;
using ChartAPI.Services.Queries;
using Microsoft.AspNetCore.SignalR;
using SqlKata;
using System.Diagnostics;

namespace ChartAPI.Services.Chart
{
    public class CalendarSummaryService : ICalendarSummaryService
    {
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly IManHourRepository _manhourRepo;
        public CalendarSummaryService(
            IManHourRepository manHourRepository,
            IHubContext<NotifyHub> hubContext)
        {
            this._manhourRepo = manHourRepository;
            this._hubContext = hubContext;
        }
        public async Task<Dictionary<string, List<CalendarSummaryDto>>> GetChart(string name, string id)
        {
            string[] costCode = {"All", "001", "002", "003", "004", "005", "006", "007", "011", "018", "021", "028", "031", "033", "037", "038", "041", "053"};
            var manHourDic = costCode.ToDictionary(x => x, y => GetManHours(y, name, id));
            await Task.WhenAll(manHourDic.Values);

            return new CalendarSummaryAssembler<ManHour>(costCode.ToDictionary(x => x, y => manHourDic[y].Result))
                .Assemble();
        }

        private async Task<List<ManHour>> GetManHours(string costCode, string name, string id)
        {
            Query query = new Query("ManHour")
                .Where("CostCode", costCode);
            if (!string.IsNullOrWhiteSpace(name))
                query.Where("Name", name);
            if (!string.IsNullOrWhiteSpace(id))
                query.Where("ID", id);
            return _manhourRepo.GetByQuery(query).ToList();
        }
    }

} 
