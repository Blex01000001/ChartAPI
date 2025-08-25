using ChartAPI.Extensions;
using System.Xml.Linq;

namespace ChartAPI.Models
{
    public class PieChartData
    {
        Dictionary<int, string> monthDict = new Dictionary<int, string>()
                {
                    { 1, "January" },
                    { 2, "February" },
                    { 3, "March" },
                    { 4, "April" },
                    { 5, "May" },
                    { 6, "June" },
                    { 7, "July" },
                    { 8, "August" },
                    { 9, "September" },
                    { 10, "October" },
                    { 11, "November" },
                    { 12, "December" }
                };

        public string Name { get; set; }           // 圖表內部名稱
        public string Title { get; set; }          // 圖表標題
        //public string[] LegendData { get; set; }   // 圖例資料
        public List<PieDataItem> Data { get; set; } // 資料本體
        public PieChartData(IGrouping<int,ManHourModel> monthGroup, string name, string groupName)
        {
            this.Name = monthGroup.Key + "月";
            this.Title = $"{name} {monthDict[monthGroup.Key]} {groupName} Distribution";
            this.Data = monthGroup
                .GroupByProperty(groupName)
                //.GroupBy(x => x.WorkNo)
                .OrderBy(dg => dg.Key)
                .Select(workNoGroup => new PieDataItem(workNoGroup.Key.ToString(), workNoGroup.Sum(s => s.Hours)))
                .ToList();
        }
    }
    public class PieDataItem
    {
        public string Name { get; set; }   // 顯示的名稱
        public double Value { get; set; }   // 對應數值
        public PieDataItem(string label, double value)
        {
            this.Name = label;
            this.Value = value;
        }
    }
}
