using ChartAPI.Extensions;
using System.Reflection;
using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.ChartBuilders.Stack
{
    public class StackChartBuilder<TModel> : ChartBuilderBase<TModel, StackChartDto>, IStackChartBuilder<TModel>
    {
        private List<StackSerie> _series = new();
        private string _chartName = "";
        public StackChartBuilder(IEnumerable<TModel> sourceData, string groupName, string sumPropName)
                : base(sourceData, groupName, sumPropName) { }
        public IStackChartBuilder<TModel> SetSeries(StackSerie series)
        {
            _series.Add((StackSerie)series.Clone());
            return this;
        }
        public IStackChartBuilder<TModel> SetName(string chartName)
        {
            _chartName = chartName;
            return this;
        }
        private string[] CreateAxisTitle()
        {
            return SourceData
                .GroupByProperty(GroupName)
                .OrderBy(g => g.Key)
                .Select(workNoGroup => workNoGroup.Key.ToString()!)
                .ToArray();
        }
        private void CreateSeries()
        {
            var groups = SourceData
            .GroupByProperty(GroupName)
            .OrderBy(g => g.Key)
            .ToList();

            foreach (var serie in _series)
            {
                // 反射取出 DataItem 的屬性
                var prop = typeof(TModel).GetProperty(serie.PropertyName, BindingFlags.Public | BindingFlags.Instance);
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
                            var hProp = typeof(TModel).GetProperty(SumPropName);
                            return Convert.ToDouble(hProp.GetValue(x));
                        }))
                    .ToArray();
            }
        }
        public override StackChartDto Build()
        {
            CreateSeries();
            return new StackChartDto(_chartName, CreateAxisTitle(), _series);
        }
    }
}
