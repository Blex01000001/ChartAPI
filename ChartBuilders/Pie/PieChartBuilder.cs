using ChartAPI.DTOs.Charts.Pie;
using ChartAPI.Extensions;

namespace ChartAPI.ChartBuilders.Pie
{
    public class PieChartBuilder<TModel> : ChartBuilderBase<TModel, PieChartDto>, IPieChartBuilder<TModel>
    {
        private List<PieItem> _data;
        public PieChartBuilder(IEnumerable<TModel> sourceData, string groupName, string sumPropName)
            : base(sourceData, groupName, sumPropName) { }
        public override PieChartDto Build()
        {
            _data = SourceData
                .GroupByProperty(GroupName)
                .OrderBy(d => d.Key)
                .Select(workNoGroup => new PieItem(workNoGroup.Key.ToString(), workNoGroup
                .Sum(s =>
                {
                    var prop = typeof(TModel).GetProperty(SumPropName);
                    return Convert.ToDouble(prop.GetValue(s));
                })))
                .ToList();
            return new PieChartDto(GroupName + " dist.", _data);
        }

    }
}
