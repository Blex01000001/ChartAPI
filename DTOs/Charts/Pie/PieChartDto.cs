using ChartAPI.Models;

namespace ChartAPI.DTOs.Charts.Pie
{
    public class PieChartDto
    {
        public string Name { get; private set; } // 圖表內部名稱
        public string Title { get; private set; } // 圖表標題
        public List<PieItem> Data { get; private set; } // 資料本體
        public PieChartDto(string title, List<PieItem> data)
        {
            //this.Name = name;
            Title = title;
            Data = data;
        }

    }
}
