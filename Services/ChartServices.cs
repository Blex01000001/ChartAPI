using ChartAPI.ChartBuilders;
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
            //新增filter條件
            var filter = new EmployeeFilter();
            if (!string.IsNullOrWhiteSpace(id)) filter.employee_id.Add(id);
            if (!string.IsNullOrWhiteSpace(name)) filter.employee_name.Add(name);
                
            string tableName = "EmpInfo9933";
            _dataRepository.UpsertData(filter, tableName);

        }
        public List<YearCalendarDto> GetCalendarData(string name, string id = null)
        {
            //新增filter條件
            var filter = new ManHourFilter()
                .Set("DateFrom", new DateTime(2015, 1, 1))
                .Set("DateTo", new DateTime(2025, 12, 31))
                .Set("Name", new List<string>() { name })
                .Set("CostCode", new List<string>() { "003", "053", "001", "011", "021", "031", "041", "002", "033", "004", "005", "006", "037", "018", "028", "038" });
            if (!string.IsNullOrWhiteSpace(id)) filter.Set("ID", new List<string>() { id });
                
            //database查詢
            string tableName = "ManHour";
            var ManHourList = _dataRepository.GetData<ManHourModel,ManHourFilter>(filter, tableName);

            //依照Calendar的資料形式分組
            return new YearCalendarBuilder(ManHourList).Build();
        }
        public List<MonthlyChartData> GetMonthlyData(int year, string name = null, string id = null)
        {
            //新增filter條件
            var filter = new ManHourFilter().Set("Year", new List<int>() { year });
            if (!string.IsNullOrWhiteSpace(id)) filter.Set("ID", new List<string>() { id });
            if (!string.IsNullOrWhiteSpace(name)) filter.Set("Name", new List<string>() { name });

            //新增Stack Series條件
            List<StackSeries> seriesCondition = new List<StackSeries>()
            {
                new StackSeries("Regular", "Regular", true),
                new StackSeries("Overtime", "Overtime", true)
            };
            //database查詢
            string tableName = "ManHour";
            var ManHourList = _dataRepository.GetData<ManHourModel, ManHourFilter>(filter, tableName);
            //依照MonthlyChartData形式分組
            return ManHourList
                .GroupBy(x => x.Month)
                .OrderBy(g => g.Key)
                .Select(monthGroup => new MonthlyChartData()
                {
                    Month = monthGroup.Key,
                    PieChartDic = new Dictionary<string, PieChartDto>()
                    {
                        { "WorkNo", new PieChartBuilder<ManHourModel>(monthGroup.ToList(), "WorkNo").Build()},
                        { "CostCode", new PieChartBuilder<ManHourModel>(monthGroup.ToList(), "CostCode").Build()}
                    },
                    StackCharts = new StackChartBuilder<ManHourModel>(monthGroup.ToList(), "WorkNo", seriesCondition)
                    .Build()
                }).ToList();
        }
        public StackChartDto<ManHourModel> GetStackChart(int year, string name = null, string id = null)
        {
            //新增filter條件
            var filter = new ManHourFilter().Set("Year", new List<int>() { year });
            if (!string.IsNullOrWhiteSpace(id)) filter.Set("ID", new List<string>() { id });
            if (!string.IsNullOrWhiteSpace(name)) filter.Set("Name", new List<string>() { name });
            //database查詢
            string tableName = "ManHour";
            var ManHourList = _dataRepository.GetData<ManHourModel, ManHourFilter>(filter, tableName);
            //新增每個Stack Series條件
            List<StackSeries> seriesCondition = new List<StackSeries>()
            {
                new StackSeries("Overtime", "Overtime", true),
                new StackSeries("Annual Paid", "CostCode", "003", "Leave"),
                new StackSeries("Compensatory", "CostCode", "053", "Leave"),
                new StackSeries("Common sick", "CostCode", "002", "Leave"),
                new StackSeries("Personal", "CostCode", "001", "Leave")
            };
            //新增Stack Chart
            return new StackChartBuilder<ManHourModel>(ManHourList, "Month", seriesCondition)
                .SetName("加班/請假年度統計")
                .Build();
        }
    }
}
