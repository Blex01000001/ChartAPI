using ChartAPI.Domain.Entities;
using ChartAPI.DTOs;
using ChartAPI.DTOs.Charts.Stack;

namespace ChartAPI.ChartBuilders
{
    //public class DeptDashboardBuilder
    //{
    //    private DeptChartDto _responseDto;
    //    private SumEntity _sumModel;
    //    public DeptDashboardBuilder(SumEntity sumModel)
    //    {
    //        _responseDto = new DeptChartDto();
    //        _sumModel = sumModel;
    //        _sumModel.SumValue = _sumModel.sumItems.OrderByDescending(x => x.SumValue).Take(30).OrderBy(x => x.SumValue).ToList();
    //    }
    //    private StackChartDto CreateStackChartDto()
    //    {
    //        //新增每個Stack Series條件
    //        List<StackSerie> baseSeries = new List<StackSerie>()
    //        {
    //            //new StackSeries("Overtime", "Overtime", true),
    //            new StackSerie("Annual Paid", "CostCode", "003", "Leave"),
    //            //new StackSeries("Compensatory", "CostCode", "053", "Leave"),
    //            //new StackSeries("Common sick", "CostCode", "002", "Leave"),
    //            //new StackSeries("Personal", "CostCode", "001", "Leave")
    //        };
    //        //新增Stack Chart

    //        var AxisTitle = _sumModel.sumItems.Select(x => x.Name).ToArray();

    //        StackChartDto stackChartDto = new StackChartDto(_sumModel.Title, AxisTitle, baseSeries);
    //        stackChartDto.Series[0].Values = _sumModel.sumItems.Select(x => x.SumValue).ToArray();

    //        return stackChartDto;
    //    }


    //    public DeptChartDto Build()
    //    {
    //        _responseDto.stackChartDtos.Add(CreateStackChartDto());

    //        return _responseDto;
    //    }
    //}
}
