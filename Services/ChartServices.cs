using ChartAPI.ChartBuilders;
using ChartAPI.DTOs;
using ChartAPI.Hubs;
using ChartAPI.Interfaces;
using ChartAPI.Models;
using ChartAPI.ResponseDto;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ChartAPI.Services
{
    public class ChartService : IChartServices
    {
        private readonly IHubContext<NotifyHub> _hubContext;
        private IDataRepository _dataRepository;
        public ChartService(IDataRepository csvRepository, IHubContext<NotifyHub> hubContext)
        {
            this._dataRepository = csvRepository;
            this._hubContext = hubContext;
        }
        //public void UpsertData(string name = null, string id = null)
        //{
        //    //新增filter條件
        //    var filter = new EmployeeFilter();
        //    if (!string.IsNullOrWhiteSpace(id)) 
        //        filter.employee_id.Add(id);
        //    if (!string.IsNullOrWhiteSpace(name)) 
        //        filter.employee_name.Add(name);

        //    string tableName = "EmpInfo9933";
        //    _dataRepository.UpsertData(filter, tableName);
        //}
        //public async Task UpsertDataByDept(string dept, string connectionId)
        //{
        //    //新增filter條件
        //    string tableName = "EmpInfo9933";
        //    var filter = new EmployeeFilter();
        //    filter.Group2.Add(dept);
        //    await _dataRepository.UpsertData(filter, tableName);
        //    await _hubContext.Clients.Client(connectionId)
        //        .SendAsync("TaskCompleted", "UpsertData Completed");
        //}

        public List<YearCalendarDto> GetCalendarData(string name, string id = null)
        {
            //新增filter條件
            var filter = new ManHourFilter()
                .Set("DateFrom", new DateTime(2012, 1, 1))
                .Set("DateTo", new DateTime(2025, 12, 31))
                .Set("Name", new List<string>() { name });
                //.Set("CostCode", new List<string>() { "003", "053", "001", "011", "021", "031", "041", "002", "033", "004", "005", "006", "007", "037", "018", "028", "038" });
            if (!string.IsNullOrWhiteSpace(id)) 
                filter.Set("ID", new List<string>() { id });
                
            //database查詢
            string tableName = "ManHour";
            var ManHourList = _dataRepository.GetData<ManHourModel,ManHourFilter>(filter, tableName);

            //依照Calendar的資料形式分組
            return new YearCalendarBuilder(ManHourList).Build();
        }
        public MonthlyChartResponseDto GetMonthlyChartResponseDto(int year, string name, string id)
        {
            //新增filter條件
            var filter = new ManHourFilter().Set("Year", new List<int>() { year });
            if (!string.IsNullOrWhiteSpace(id)) 
                filter.Set("ID", new List<string>() { id });
            if (!string.IsNullOrWhiteSpace(name)) 
                filter.Set("Name", new List<string>() { name });
            
            //database查詢
            string tableName = "ManHour";
            var ManHourList = _dataRepository.GetData<ManHourModel, ManHourFilter>(filter, tableName);
            return new MonthlyChartBuilder(ManHourList).Build();
        }
        public DeptChartDto GetDeptYearChartDto(string dept)
        {
            //database查詢
            var filter = new ManHourFilter();
            filter.Group2.Add(dept);
            string tableName = "ManHour";

            SumModel sumItem = _dataRepository.GetSumData(
                            year: 2025,
                            group2Filter: "管線",
                            reportTitle: "2025年管線部門 - 加班總工時",
                            isOvertime: true
                        );
            return new DeptDashboardBuilder(sumItem).Build();
        }
        public YearChartDto GetYearChartDto()
        {
            var filter = new ManHourFilter();
            string tableName = "ManHour";

            var ManHourList = _dataRepository.GetData<ManHourModel, ManHourFilter>(filter, tableName);
            return new YearChartBuilder(ManHourList).Build();
        }
    }
}
