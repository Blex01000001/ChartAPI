namespace ChartAPI.DTOs.Charts.Pie
{
    public class PieItem
    {
        public string Name { get; set; }   // 顯示的名稱
        public double Value { get; set; }   // 對應數值
        public PieItem(string label, double value)
        {
            Name = label;
            Value = value;
        }

    }
}
