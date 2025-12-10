using ChartAPI.DTOs.Charts.Pie;
using ChartAPI.Extensions;
using ChartAPI.Models;
using System.Collections.Generic;

namespace ChartAPI.ChartBuilders.Pie
{
    public class PieChartBuilder<T> : ChartBuilderBase<T>, IChartBuilder<PieChartDto>
    {
        //private readonly IEnumerable<T> _sourceData;
        //private readonly string _groupName;
        //private readonly string _sumPropName;
        private List<PieItem> _data;
        public PieChartBuilder(IEnumerable<T> sourceData, string groupName, string sumPropName)
            : base(sourceData, groupName, sumPropName) { }
        public PieChartDto Build()
        {
            _data = SourceData
                .GroupByProperty(GroupName)
                .OrderBy(d => d.Key)
                .Select(workNoGroup => new PieItem(workNoGroup.Key.ToString(), workNoGroup
                .Sum(s =>
                {
                    var prop = typeof(T).GetProperty(SumPropName);
                    return Convert.ToDouble(prop.GetValue(s));
                })))
                .ToList();
            return new PieChartDto(GroupName + " dist.", _data);
        }

    }
}
