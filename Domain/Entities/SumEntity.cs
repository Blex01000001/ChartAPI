namespace ChartAPI.Domain.Entities
{
    public class SumEntity_
    {
        public int Year { get; set; }
        public string Title { get; set; }
        public List<SumEntity> sumItems { get; set; }

    }
    public class SumEntity()
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public double SumValue { get; set; }
    }
}
