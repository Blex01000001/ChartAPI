namespace ChartAPI.Models
{
    public class ManHourFilter
    {
        // 日期用 From/To
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // 其餘一律用 List 作為多重過濾
        public List<string> Name { get; } = new List<string>();
        public List<string> ID { get; } = new List<string>();
        public List<int> Year { get; } = new List<int>();
        public List<int> Month { get; } = new List<int>();
        public List<DateTime> Weekend { get; } = new List<DateTime>();
        public List<string> WorkNo { get; } = new List<string>();
        public List<string> PH { get; } = new List<string>();
        public List<string> DP { get; } = new List<string>();
        public List<string> CostCode { get; } = new List<string>();
        public List<string> CL { get; } = new List<string>();
        public List<string> UnitArea { get; } = new List<string>();
        public List<string> SystemNo { get; } = new List<string>();
        public List<string> EquipNo { get; } = new List<string>();
        public List<string> SE { get; } = new List<string>();
        public List<string> REWK { get; } = new List<string>();
        public List<DayOfWeek> DayofWeek { get; } = new List<DayOfWeek>();
        public List<double> Hours { get; } = new List<double>();
        public List<bool> Regular { get; } = new List<bool>();
        public List<bool> Overtime { get; } = new List<bool>();
        public List<DateTime> Updated { get; } = new List<DateTime>();
        public List<string> Position { get; } = new List<string>();
        public List<string> Group1 { get; } = new List<string>();
        public List<string> Group2 { get; } = new List<string>();
        public List<string> Group3 { get; } = new List<string>();
        public List<string> Group4 { get; } = new List<string>();
    }
}
