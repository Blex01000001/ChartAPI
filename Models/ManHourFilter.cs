namespace ChartAPI.Models
{
    public class ManHourFilter
    {
        // 日期用 From/To
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // 其餘一律用 List 作為多重過濾
        public List<string> Name { get; private set; } = new List<string>();
        public List<string> ID { get; private set; } = new List<string>();
        public List<int> Year { get; private set; } = new List<int>();
        public List<int> Month { get; private set; } = new List<int>();
        public List<DateTime> Weekend { get; private set; } = new List<DateTime>();
        public List<string> WorkNo { get; private set; } = new List<string>();
        public List<string> PH { get; private set; } = new List<string>();
        public List<string> DP { get; private set; } = new List<string>();
        public List<string> CostCode { get; private set; } = new List<string>();
        public List<string> CL { get; private set; } = new List<string>();
        public List<string> UnitArea { get; private set; } = new List<string>();
        public List<string> SystemNo { get; private set; } = new List<string>();
        public List<string> EquipNo { get; private set; } = new List<string>();
        public List<string> SE { get; private set; } = new List<string>();
        public List<string> REWK { get; private set; } = new List<string>();
        public List<DayOfWeek> DayofWeek { get; private set; } = new List<DayOfWeek>();
        public List<double> Hours { get; private set; } = new List<double>();
        public List<bool> Regular { get; private set; } = new List<bool>();
        public List<bool> Overtime { get; private set; } = new List<bool>();
        public List<DateTime> Updated { get; private set; } = new List<DateTime>();
        public List<string> Position { get; private set; } = new List<string>();
        public List<string> Group1 { get; private set; } = new List<string>();
        public List<string> Group2 { get; private set; } = new List<string>();
        public List<string> Group3 { get; private set; } = new List<string>();
        public List<string> Group4 { get; private set; } = new List<string>();
        public ManHourFilter Set<TValue>(string propName, TValue value)
        {
            var prop = typeof(ManHourFilter).GetProperty(propName);
            if (prop == null) throw new ArgumentException($"屬性 {propName} 不存在");
                prop.SetValue(this, value);
            return this;
        }
    }
}
