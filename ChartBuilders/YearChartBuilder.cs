using ChartAPI.DTOs;
using ChartAPI.Models;

namespace ChartAPI.ChartBuilders
{
    public class YearChartBuilder
    {
        public YearChartBuilder(IEnumerable<ManHourModel> manHourList)
        {

        }
        public YearChartDto Build()
        {
            return new YearChartDto();
        }
    }
}
