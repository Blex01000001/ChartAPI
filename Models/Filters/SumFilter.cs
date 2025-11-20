namespace ChartAPI.Models.Filters
{
    public class SumFilter : BaseFilter
    {
        public int Year
        {
            get => TryGet<int>(nameof(Year));
            set => Set(nameof(Year), value);
        }

        public string Group2Contains
        {
            get => TryGet<string>(nameof(Group2Contains));
            set => Set(nameof(Group2Contains), value);
        }

        public bool OnlyOvertime
        {
            get => TryGet<bool>(nameof(OnlyOvertime));
            set => Set(nameof(OnlyOvertime), value);
        }

        public List<string> CostCodes
        {
            get => TryGet<List<string>>(nameof(CostCodes)) ?? new();
            set => Set(nameof(CostCodes), value);
        }

        public bool JoinEmpInfo
        {
            get => TryGet<bool>(nameof(JoinEmpInfo));
            set => Set(nameof(JoinEmpInfo), value);
        }

        public List<string> GroupBy
        {
            get => TryGet<List<string>>(nameof(GroupBy)) ?? new();
            set => Set(nameof(GroupBy), value);
        }

        public List<string> OrderBy
        {
            get => TryGet<List<string>>(nameof(OrderBy)) ?? new();
            set => Set(nameof(OrderBy), value);
        }
    }
}
