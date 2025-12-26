using ChartAPI.Domain.Entities;
using ChartAPI.DTOs;
using ChartAPI.Extensions;

namespace ChartAPI.Assemblers.Charts
{
    public class CalendarSummaryAssembler<T>
    {
        private Dictionary<string, List<ManHour>> _manHourDic;
        public CalendarSummaryAssembler(Dictionary<string, List<ManHour>> manHourDic)
        {
            _manHourDic = manHourDic;
        }
        public Dictionary<string, List<CalendarSummaryDto>> Assemble()
        {
            ConsoleExtensions.WriteLineWithTime($"AssembleCalendarSummary");
            List<ManHour> total = new List<ManHour>();
            foreach (var item in _manHourDic)
            {
                total.AddRange(item.Value);
            }
            _manHourDic["All"] = total;

            return _manHourDic.ToDictionary(x => x.Key, 
                y => y.Value
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
                .OrderByDescending(x => x.Year)
                .ToList());
        }
    }
}
