using ChartAPI.DTOs;
using ChartAPI.Models;

namespace ChartAPI.ChartBuilders
{
    public class YearCalendarBuilder
    {
        private readonly IEnumerable<ManHourModel> _sourceData;

        public YearCalendarBuilder(IEnumerable<ManHourModel> sourceData)
        {
            this._sourceData = sourceData;
        }
        public List<CalendarSummaryDto> Build()
        {
            var test = _sourceData.GroupBy(x => x.Date);
            return _sourceData
                .GroupBy(x => x.Date.Year)
                .OrderBy(g => g.Key)
                .Select(yearGroup => new CalendarSummaryDto
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
        }
    }
}
