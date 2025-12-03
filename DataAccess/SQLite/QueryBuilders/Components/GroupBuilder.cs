namespace ChartAPI.DataAccess.SQLite.QueryBuilders.Components
{
    public class GroupBuilder
    {
        private readonly List<string> _fields = new();

        public void Add(string field) => _fields.Add(field);

        //public string Build() { ... }
    }
}
