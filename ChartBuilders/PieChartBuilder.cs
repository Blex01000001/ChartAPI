using ChartAPI.DTOs;
using ChartAPI.Extensions;
using ChartAPI.Models;
using System.Collections.Generic;

namespace ChartAPI.ChartBuilders
{
    public class PieChartBuilder<T>
    {
        private readonly IEnumerable<T> _sourceData;
        private readonly string _groupName;
        private List<PieItem> _data;
        //Dictionary<int, string> monthDict = new Dictionary<int, string>()
        //        {
        //            { 1, "Jan." },
        //            { 2, "Feb." },
        //            { 3, "Mar." },
        //            { 4, "Apr." },
        //            { 5, "May." },
        //            { 6, "Jun." },
        //            { 7, "Jul." },
        //            { 8, "Aug." },
        //            { 9, "Sep." },
        //            { 10, "Oct." },
        //            { 11, "Nov." },
        //            { 12, "Dec." }
        //        };
        public PieChartBuilder(IEnumerable<T> sourceData, string groupName)
        {
            this._sourceData = sourceData;
            this._groupName = groupName;
        }
        private void GenerateData()
        {
            this._data = _sourceData
                .GroupByProperty(_groupName)
                .OrderBy(d => d.Key)
                .Select(workNoGroup => new PieItem(workNoGroup.Key.ToString(), workNoGroup
                .Sum(s =>
                {
                    var prop = typeof(T).GetProperty("Hours");
                    return Convert.ToDouble(prop.GetValue(s));
                })))
                .ToList();
        }
        public PieChartDto Build()
        {
            GenerateData();
            return new PieChartDto (_groupName + " dist.", _data);
        }

    }
}
