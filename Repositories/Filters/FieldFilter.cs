namespace ChartAPI.Repositories.Filters
{
    public class FieldFilter
    {
        public string FieldName { get; set; }
        public FilterFieldType FieldType { get; set; }
        public object Value { get; set; }

        public FieldFilter(string name, FilterFieldType type, object value)
        {
            FieldName = name;
            FieldType = type;
            Value = value;
        }
    }
}
