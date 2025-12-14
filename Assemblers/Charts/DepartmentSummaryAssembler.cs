using ChartAPI.DTOs.Charts.Stack;
using ChartAPI.Models;

namespace ChartAPI.Assemblers.Charts
{
    public class DepartmentSummaryAssembler
    {
        private IEnumerable<SumItem> _sourceDatas;
        private string _deptName = string.Empty;

        public DepartmentSummaryAssembler(IEnumerable<SumItem> datas)
        {
            _sourceDatas = datas.OrderBy(x => x.SumValue);
        }
        public DepartmentSummaryAssembler SetDept(string deptName)
        {
            this._deptName = deptName;
            return this;
        }
        public StackChartDto Assemble()
        {
            //return new StackChartBuilder<T>(_sourceDatas, "Name", "SumValue")
            //    .SetName($"{_deptName} 加班年度統計")
            //    .SetSeries(new StackSerie("Overtime", "SumValue", true))
            //    .Build();
            string[] axisTitle = _sourceDatas.Select(x => x.Name).ToArray();
            StackSerie stackSerie = new StackSerie("name", "", "");
            stackSerie.Values = _sourceDatas.Select(x => x.SumValue).ToArray();
            var series = new List<StackSerie>() { stackSerie };
            return new StackChartDto("加班年度統計", axisTitle, series);
        }
    }
}
