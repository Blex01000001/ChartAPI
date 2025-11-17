namespace ChartAPI.DTOs
{
    public class StackSerie : ICloneable
    {
        public string Name { get; set; }// 這個系列的名稱（對應 legend）
        public string PropertyName { get; set; }// 要篩選的欄位名稱
        public object FilterValue { get; set; }// 篩選的值
        public double[] Values { get; set; }// 對應每個 category 的值
        public string Stack { get; set; }// 堆疊組別名稱
        public StackSerie(string seriesName, string propertyName, object filterValue, string stack = "total")
        {
            this.Name = seriesName;
            this.PropertyName = propertyName;
            this.FilterValue = filterValue;
            this.Stack = stack;
        }

        public object Clone()
        {
            return new StackSerie(Name, PropertyName, FilterValue, Stack);
        }
    }
}
