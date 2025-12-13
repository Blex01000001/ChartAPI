using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.Services.Chart
{
    public interface IDepartmentSummaryService
    {
        StackChartDto GetChart(int year, string deptName);
    }
}
