using ChartAPI.DTOs;

namespace ChartAPI.Assemblers.Charts
{
    public class CalendarSummaryAssembler<T>
    {
        private IEnumerable<T> _sourceDatas;
        public CalendarSummaryAssembler(IEnumerable<T> datas)
        {
            _sourceDatas = datas;
        }
        public List<CalendarSummaryDto> Assemble()
        {
            return new List<CalendarSummaryDto>();
        }
    }
}
