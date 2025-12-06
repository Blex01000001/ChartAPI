using ChartAPI.Extensions;
using System.Reflection;
using System.Xml.Linq;
using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.ChartBuilders.Stack
{
    public class StackChartBuilder<T> : IStackChartBuilder<T>
    {
        private IEnumerable<T> _sourceData;
        private List<StackSerie> _series;
        private string _chartName = "加班/請假年度統計";
        private readonly string _groupName;
        private readonly string _sumPropName;
        public StackChartBuilder(IEnumerable<T> sourceData, string groupName, string sumPropName)
        {
            _sourceData = sourceData;
            _groupName = groupName;
            _sumPropName = sumPropName;
        }
        public StackChartBuilder<T> SetSeries(List<StackSerie> series)
        {
            _series = series.Select(x => (StackSerie)x.Clone()).ToList();
            return this;
        }

        public StackChartBuilder<T> SetName(string chartName)
        {
            _chartName = chartName;
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

            foreach (var serie in _series)
            {
                // 反射取出 DataItem 的屬性
                var prop = typeof(T).GetProperty(serie.PropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                    throw new ArgumentException($"找不到屬性 {serie.PropertyName}");

                // 計算每個分群的 Sum(Hours)，條件為 prop.Value == FilterValue
                serie.Values = groups
                    .Select(g => g
                        .Where(item =>
                        {
                            var val = prop.GetValue(item);
                            // 為了安全先判 null，再用 Equals 比對
                            return val != null && val.Equals(serie.FilterValue);
                        })
                        .Sum(x =>
                        {
                            // 假設每筆都有 Hours 屬性
                            var hProp = typeof(T).GetProperty(_sumPropName);
                            return Convert.ToDouble(hProp.GetValue(x));
                        }))
                    .ToArray();
            }
        }
        public StackChartDto Build()
        {
            CreateSeries();
            return new StackChartDto(_chartName, CreateAxisTitle(), _series);
        }
    }
}
