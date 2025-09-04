using ChartAPI.Extensions;
using ChartAPI.DTOs;
using System.Reflection;
using System.Xml.Linq;

namespace ChartAPI.ChartBuilders
{
    public class StackChartBuilder<T>
    {
        private readonly IEnumerable<T> _sourceData;
        private readonly string _groupName;
        private readonly List<StackSeries> _series;
        private string _chartName;
        public StackChartBuilder(IEnumerable<T> sourceData, string groupName, List<StackSeries> series)
        {
            _sourceData = sourceData;
            _groupName = groupName;
            _series = series;
        }
        public StackChartBuilder<T> SetName(string chartName)
        {
            this._chartName = chartName;
            return this;
        }
        private string[] CreateAxisTitle()
        {
            return _sourceData
                .GroupByProperty(_groupName)
                .OrderBy(g => g.Key)
                .Select(workNoGroup => workNoGroup.Key.ToString()!)
                .ToArray();
        }
        private void CreateSeries()
        {
            var groups = _sourceData
            .GroupByProperty(_groupName)
            .OrderBy(g => g.Key)
            .ToList();

            var result = new List<StackSeries>();
            foreach (var series in _series)
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
        public StackChartDto<T> Build()
        {
            CreateSeries();
            return new StackChartDto<T>(_chartName, CreateAxisTitle(), _series);
        }
    }
}
