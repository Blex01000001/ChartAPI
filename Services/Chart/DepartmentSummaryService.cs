using ChartAPI.Assemblers.Charts;
using ChartAPI.DataAccess.Interfaces;
using ChartAPI.DTOs.Charts.Stack;
using ChartAPI.Hubs;
using ChartAPI.Models;
using ChartAPI.Services.Queries;
using Microsoft.AspNetCore.SignalR;
using SqlKata;
using System.Xml.Linq;

namespace ChartAPI.Services.Chart
{
    public class DepartmentSummaryService : IDepartmentSummaryService
    {
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly ISumItemQueryService _sumItemQueryService;
        public DepartmentSummaryService(
            ISumItemQueryService sumItemQueryService,
            IHubContext<NotifyHub> hubContext)
        {
            this._sumItemQueryService = sumItemQueryService;
            this._hubContext = hubContext;
        }
        public StackChartDto GetChart(int year, string deptName)
        {
            var query = new Query("ManHour AS mh")
                .Join("EmpInfo9933 AS emp", "mh.ID", "emp.employee_id") // INNER JOIN
                .Select("mh.ID","mh.Name")
                .SelectRaw("SUM(mh.Hours) AS SumValue")
                .Where("mh.Year", year)
                .Where("mh.Overtime", 1)
                .WhereLike("emp.Group2", $"%{deptName}%")
                .GroupBy("mh.ID", "mh.Name")
                .OrderByDesc("SumValue");

            IEnumerable<SumItem> sumItems = _sumItemQueryService.GetByQuery(query).ToList();

            return new DepartmentSummaryAssembler(sumItems.Take(30))
                .SetDept(deptName)
                .Assemble();
        }
    }
}
