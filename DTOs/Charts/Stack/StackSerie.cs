namespace ChartAPI.DTOs.Charts.Stack
{
    public class StackSerie : ICloneable
    {
        public string Name { get; set; }// 這個系列的名稱（對應 legend）
        public string PropertyName { get; set; }// 要篩選的欄位名稱
        public object FilterValue { get; set; }// 篩選的值
        public double[] Values { get; set; }// 對應每個 category 的值
        public string Stack { get; set; }// 堆疊組別名稱
        /// <summary>
        /// 設定每個系列的資料設定
        /// </summary>
        /// <param name="seriesName">系列名稱，與圖例相同</param>666
        /// <param name="propertyName">篩選的屬性名稱</param>
        /// <param name="filterValue">篩選屬性的值</param>
        /// <param name="stack">堆疊的名稱，預設為total</param>
        public StackSerie(string seriesName, string propertyName, object filterValue, string stack = "total")
        {
            Name = seriesName;
            PropertyName = propertyName;
            FilterValue = filterValue;
            Stack = stack;
        }

        public object Clone()
        {
            return new StackSerie(Name, PropertyName, FilterValue, Stack);
        }
    }
}
