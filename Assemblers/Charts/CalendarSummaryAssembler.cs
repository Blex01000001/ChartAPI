using ChartAPI.DTOs;
using ChartAPI.Models;

namespace ChartAPI.Assemblers.Charts
{
    public class CalendarSummaryAssembler<T>
    {
        private Dictionary<string, List<ManHourModel>> _manHourDic;
        public CalendarSummaryAssembler(Dictionary<string, List<ManHourModel>> manHourDic)
        {
            _manHourDic = manHourDic;
        }
        public List<CalendarSummaryDto> Assemble()
        {
            List<ManHourModel> total = new List<ManHourModel>();
            foreach (var item in _manHourDic)
            {
                total.AddRange(item.Value);
            }
            _manHourDic["All"] = total;

            return _manHourDic["All"]
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
