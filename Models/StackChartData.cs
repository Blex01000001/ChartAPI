using ChartAPI.Extensions;

namespace ChartAPI.Models
{
    public class StackChartData
    {
        public string Name { get; set; }
        //public string Title { get; set; }

        private string _groupName = "WorkNo";
        private string _Regular;
        private string _Overtime;

        public string[] AxisTitle { get; set; }
        //public string YAxisName { get; set; }
        //public string[] LegendData { get; set; }
        public List<Series> Series { get; set; } // 每個 bar 類別
        public StackChartData(IGrouping<int, ManHourModel> monthGroup, string name, string groupName)
        {
            this.Name = monthGroup.Key + "月";
            this.AxisTitle = monthGroup
                .GroupByProperty(groupName)
                .OrderBy(dg => dg.Key)
                .Select(workNoGroup => workNoGroup.Key.ToString())
                .ToArray();
            this.Series = new List<Series>()
                {
                    new Series("Regular")
                    {
                        //Values = monthGroup.GroupBy(x => x.WorkNo).OrderBy(g => g.Key)
                        Values = monthGroup.GroupByProperty(groupName).OrderBy(g => g.Key)
                        .Select(g => g.Where(x => x.Regular).Sum(x => x.Hours)).ToArray()
                    },
                    new Series("Overtime")
                    {
                        Values = monthGroup.GroupByProperty(groupName).OrderBy(g => g.Key)
                        .Select(g => g.Where(x => x.Overtime).Sum(x => x.Hours)).ToArray()
                    }
                };

        }

    }

    public class Series
    {
        public string Name { get; set; }     // 這個系列的名稱（對應 legend）
        public double[] Values { get; set; } // 對應每個 category 的值
        public Series(string name)
        {
            this.Name = name;
            //this.Values = doubles;
        }
    }
}
