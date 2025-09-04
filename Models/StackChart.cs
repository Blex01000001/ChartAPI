using ChartAPI.Extensions;
using System.Reflection;

namespace ChartAPI.Models
{
    public class StackChart<T>
    {
        public string Name { get; private set; }
        public string[] AxisTitle { get; private set; }
        public List<StackSeries> Series { get; private set; }

        private IEnumerable<T> _sourceData;
        private string _groupName;
        public StackChart(IEnumerable<T> source, string groupName, List<StackSeries> stackSeries)
        {
            _sourceData = source;
            _groupName = groupName;
            Series = stackSeries;
            CreateAxisTitle();
            CreateSeries();
        }
        private void CreateAxisTitle()
        {
            this.AxisTitle = _sourceData
                .GroupByProperty(_groupName)
                .OrderBy(g => g.Key)
                .Select(workNoGroup => workNoGroup.Key.ToString()!)
                .ToArray();

        }
        private void CreateSeries()
        {
            var groups = _sourceData
            .GroupByProperty(_groupName)    // 假設你已經有這個擴充
            .OrderBy(g => g.Key)
            .ToList();

            var result = new List<StackSeries>();
            foreach (var series in Series)
            {
                // 反射取出 DataItem 的屬性
                var prop = typeof(T).GetProperty(series.PropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                    throw new ArgumentException($"找不到屬性 {series.PropertyName}");

                // 計算每個分群的 Sum(Hours)，條件為 prop.Value == FilterValue
                var values = groups
                    .Select(g => g
                        .Where(item =>
                        {
                            var val = prop.GetValue(item);
                            // 為了安全先判 null，再用 Equals 比對
                            return val != null && val.Equals(series.FilterValue);
                        })
                        .Sum(x =>
                        {
                            // 假設每筆都有 Hours 屬性
                            var hProp = typeof(T).GetProperty("Hours");
                            return Convert.ToDouble(hProp.GetValue(x));
                        }))
                    .ToArray();

                series.Values = values;
            }
        }
        public StackChart<T> SetName(string chartName)
        {
            Name = chartName;
            return this;
        }
    }
    public class StackSeries
    {
        public string Name { get; set; }     // 這個系列的名稱（對應 legend）
        public string PropertyName { get; set; }
        public object FilterValue { get; set; }
        public double[] Values { get; set; } // 對應每個 category 的值
        public string Stack { get; set; }
        public StackSeries(string seriesName, string propertyName, object filterValue, string stack = "total")
        {
            this.Name = seriesName;
            this.PropertyName = propertyName;
            this.FilterValue = filterValue;
            this.Stack = stack;
        }
    }
}
