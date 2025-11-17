using ChartAPI.DTOs;
using ChartAPI.Models;
using ChartAPI.ResponseDto;

namespace ChartAPI.Interfaces
{
    public interface IChartServices
    {
        List<YearCalendarDto> GetCalendarData(string name, string id);
        MonthlyChartResponseDto GetMonthlyChartResponseDto(int year, string name, string id);
        //void UpsertData(string name = null, string id = null);
        //Task UpsertDataByDept(string dept, string connectionId);
        DeptChartDto GetDeptYearChartDto(string dept);
    }
}
