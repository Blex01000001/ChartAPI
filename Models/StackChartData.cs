using ChartAPI.Extensions;
using System.Reflection;

namespace ChartAPI.Models
{
    public class StackChartData<T>
    {
        public string Name { get; set; }
        public string[] AxisTitle { get; set; }
        public List<Series> Series { get; set; }

        private IEnumerable<T> _sourceData;
        private string _groupName;
        private List<StackSeries> _stackSeries;
        // 每個 bar 類別
        public StackChartData(IEnumerable<T> source, string groupName, List<StackSeries> stackSeries)
        {
            _sourceData = source;
            _groupName = groupName;
            _stackSeries = stackSeries;

            this.AxisTitle = _sourceData
                .GroupByProperty(groupName)
                .OrderBy(g => g.Key)
                .Select(workNoGroup => workNoGroup.Key.ToString())
                .ToArray();
            this.Series = SeriesGenerator();
        }
        private List<Series> SeriesGenerator()
        {
            var groups = _sourceData
            .GroupByProperty(_groupName)    // 假設你已經有這個擴充
            .OrderBy(g => g.Key)
            .ToList();

            var result = new List<Series>();
            foreach (var stackSerie in _stackSeries)
            {
                // 反射取出 DataItem 的屬性
                var prop = typeof(T).GetProperty(stackSerie.PropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                    throw new ArgumentException($"找不到屬性 {stackSerie.PropertyName}");

                // 計算每個分群的 Sum(Hours)，條件為 prop.Value == FilterValue
                var values = groups
                    .Select(g => g
                        .Where(item =>
                        {
                            var val = prop.GetValue(item);
                            // 為了安全先判 null，再用 Equals 比對
                            return val != null && val.Equals(stackSerie.FilterValue);
                        })
                        .Sum(x =>
                        {
                            // 假設每筆都有 Hours 屬性
                            var hProp = typeof(T).GetProperty("Hours");
                            return Convert.ToDouble(hProp.GetValue(x));
                        }))
                    .ToArray();

                result.Add(new Series(stackSerie.SeriesName)
                {
                    Values = values
                });
            }
            return result;
        }
    }
    public class Series
    {
        public string Name { get; set; }     // 這個系列的名稱（對應 legend）
        public double[] Values { get; set; } // 對應每個 category 的值
        public Series(string name)
        {
            this.Name = name;
        }
    }
    public class StackSeries
    {
        public string SeriesName { get; set; }
        public string PropertyName { get; set; }
        public object FilterValue { get; set; }

        public StackSeries(string seriesName, string propertyName, object filterValue)
        {
            SeriesName = seriesName;
            PropertyName = propertyName;
            FilterValue = filterValue;
        }
    }

}
