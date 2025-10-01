namespace ChartAPI.Models
{
    public class SumModel
    {
        public int Year { get; set; }
        public string Title { get; set; }
        public List<SumItem> sumItems { get; set; }

    }
    public class SumItem()
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
    }
}
