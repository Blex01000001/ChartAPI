using ChartAPI.DTOs;
using ChartAPI.Interfaces;
using ChartAPI.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Collections.Generic;
using System.Linq;

namespace ChartAPI.Services
{
    public class ChartService : IChartServices
    {
        private IDataRepository _dataRepository;
        public ChartService(IDataRepository csvRepository)
        {
            this._dataRepository = csvRepository;
        }
        public void UpsertData(string name = null, string id = null)
        {
            var filter = new EmployeeFilter();
            if (!string.IsNullOrWhiteSpace(id))
                filter.employee_id.Add(id);
            if (!string.IsNullOrWhiteSpace(name))
                filter.employee_name.Add(name);
            string tableName = "EmpInfo9933";
            _dataRepository.UpsertData(filter, tableName);

        }
        public List<YearCalendarDataDto> GetCalendarData(string name, string id = null)
        {
            //新增filter條件
            var filter = new ManHourFilter
            {
                DateFrom = new DateTime(2015, 1, 1),
                DateTo = new DateTime(2025, 12, 31)
            };
            if (!string.IsNullOrWhiteSpace(id))
                filter.ID.Add(id);
            filter.Name.AddRange(new[] { name });
            filter.CostCode.AddRange(new[] { "003", "053", "001", "011", "021", "031", "041", "002", "033", "004", "005", "006", "037", "018", "028", "038" });
            string tableName = "ManHour";
            var ManHourList = _dataRepository.GetData<ManHourModel,ManHourFilter>(filter, tableName);

            //依照Calendar的資料形式分組
            var result = ManHourList
                .GroupBy(x => x.Date.Year)
                .OrderBy(g => g.Key)
                .Select(yearGroup => new YearCalendarDataDto
                {
                    Year = yearGroup.Key,
                    Data = yearGroup
                        .GroupBy(x => x.Date.ToString("yyyy-MM-dd"))
                        .OrderBy(dg => dg.Key)
                        .Select(dg => new object[]
                        {
                            dg.Key,             // 0: "yyyy-MM-dd"
                            dg.Sum(m => m.Hours) // 1: 加總後的 Hours
                        })
                        .ToList()
                })
                .ToList();
            return result;
        }
        public List<MonthlyChartData> GetMonthlyData(int year, string name = null, string id = null)
        {
            //新增filter條件
            var filter = new ManHourFilter();
            if (!string.IsNullOrWhiteSpace(id))
                filter.ID.Add(id);
            if (!string.IsNullOrWhiteSpace(name))
                filter.Name.Add(name);
            filter.Year.Add(year);

            //新增Stack Series條件
            List<StackSeries> seriesCondition = new List<StackSeries>()
            {
                new StackSeries("Regular", "Regular", true),
                new StackSeries("Overtime", "Overtime", true)
            };

            string tableName = "ManHour";
            var ManHourList = _dataRepository.GetData<ManHourModel, ManHourFilter>(filter, tableName);

            //依照資料形式分組
            List<MonthlyChartData> result = ManHourList
                .GroupBy(x => x.Month)
                .OrderBy(g => g.Key)
                .Select(monthGroup => new MonthlyChartData()
                {
                    Month = monthGroup.Key,
                    PieChartDic = new Dictionary<string, PieChartData>()
                    {
                        { "WorkNo", new PieChartData(monthGroup, name, "WorkNo")},
                        { "CostCode", new PieChartData(monthGroup, name, "CostCode")}
                    },
                    StackCharts = new StackChart<ManHourModel>(monthGroup.ToList(), "WorkNo", seriesCondition),
                }).ToList();

            return result;
        }
        public StackChart<ManHourModel> GetStackChart(int year, string name = null, string id = null)
        {
            //新增filter條件
            var filter = new ManHourFilter();
            if (!string.IsNullOrWhiteSpace(id))
                filter.ID.Add(id);
            if (!string.IsNullOrWhiteSpace(name))
                filter.Name.Add(name);
            filter.Year.Add(year);
            string tableName = "ManHour";
            var ManHourList = _dataRepository.GetData<ManHourModel, ManHourFilter>(filter, tableName);

            //新增Stack Series條件
            List<StackSeries> seriesCondition = new List<StackSeries>()
            {
                new StackSeries("Overtime", "Overtime", true),
                new StackSeries("Annual Paid", "CostCode", "003", "Leave"),
                new StackSeries("Compensatory", "CostCode", "053", "Leave"),
                new StackSeries("Common sick", "CostCode", "002", "Leave"),
                new StackSeries("Personal", "CostCode", "001", "Leave")
            };
            //新增Stack Chart條件
            return new StackChart<ManHourModel>(ManHourList, "Month", seriesCondition)
                .SetName("加班/請假年度統計");
        }
    }
}
