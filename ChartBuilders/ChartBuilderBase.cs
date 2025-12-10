namespace ChartAPI.ChartBuilders
{
    public class ChartBuilderBase<TModel>
    {
        protected readonly IEnumerable<TModel> SourceData;
        protected readonly string GroupName;
        protected readonly string SumPropName;
        protected ChartBuilderBase(IEnumerable<TModel> sourceData, string groupName, string sumPropName)
        {
            SourceData = sourceData;
            GroupName = groupName;
            SumPropName = sumPropName;
        }
    }
}
