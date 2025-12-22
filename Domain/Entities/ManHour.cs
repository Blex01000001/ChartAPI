namespace ChartAPI.Domain.Entities
{
    public class ManHour
    {
        public string Name { get; set; }//0
        public string ID { get; set; }
        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime Weekend { get; set; }
        public string WorkNo { get; set; }
        public string PH { get; set; }//5
        public string DP { get; set; }
        public string CostCode { get; set; }
        public string CL { get; set; }
        public string UnitArea { get; set; }
        public string SystemNo { get; set; }//10
        public string EquipNo { get; set; }
        public string SE { get; set; }
        public string REWK { get; set; }
        public DayOfWeek DayofWeek { get; set; }//14
        public double Hours { get; set; }//15
        public bool Regular { get; set; }
        public bool Overtime { get; set; }
        public DateTime Updated { get; set; }//18
    }
}
