using ChartAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using ChartAPI.DTOs;
using ChartAPI.Services.Queries;
using ChartAPI.DataAccess.SQLite.QueryBuilders;
using ChartAPI.Assemblers.Charts;
using SqlKata;
using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Domain.Entities;
using ChartAPI.Extensions;

namespace ChartAPI.Services.Chart
{
    public class AnnualSummaryService : IAnnualSummaryService
    {
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly IManHourRepository _manhourRepo;

        public AnnualSummaryService(
            IManHourRepository manHourRepository,
            IHubContext<NotifyHub> hubContext)
        {
            this._manhourRepo = manHourRepository;
            this._hubContext = hubContext;
        }
        public AnnualSummaryDto GetChart(int year, string name, string id)
        {
            Query query = new Query("ManHour")
                .Where("Year", year);
            if (!string.IsNullOrWhiteSpace(name))
                query.Where("Name", name);
            if (!string.IsNullOrWhiteSpace(id))
                query.Where("ID", id);

            List<ManHour> manhours = _manhourRepo.GetByQuery(query).ToList();
            ConsoleExtensions.WriteLineWithTime($"AssembleAnnualSummary");
            return new AnnualSummaryAssembler<ManHour>(manhours)
                .SetName(name)
                .Assemble();
        }

    }
}
