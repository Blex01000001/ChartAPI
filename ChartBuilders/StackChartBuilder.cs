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
        private readonly List<StackSerie> _series;
        private string _chartName;
        private string _sumPropName;
        public StackChartBuilder(IEnumerable<T> sourceData, string groupName, string sumPropName, List<StackSerie> series)
        {
            _sourceData = sourceData;
            _groupName = groupName;
            _series = series.Select(x => (StackSerie)x.Clone()).ToList();
            _sumPropName = sumPropName;
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
        public StackChartDto<T> Build()
        {
            CreateSeries();
            return new StackChartDto<T>(_chartName, CreateAxisTitle(), _series);
        }
    }
}
