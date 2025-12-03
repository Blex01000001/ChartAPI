using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace ChartAPI.DataAccess.SQLite.QueryBuilders.Components
{
    public sealed class WhereBuilder
    {
        private readonly List<string> _conditions = new();
        private readonly List<SQLiteParameter> _parameters = new();

        public int NextParameterIndex => _parameters.Count;

        public void Add(ExpressionResult fragment)
        {
            if (string.IsNullOrWhiteSpace(fragment.SqlCondition))
                return;

            _conditions.Add(fragment.SqlCondition);
            _parameters.AddRange(fragment.Parameters);
        }

        public string Build()
        {
            if (_conditions.Count == 0)
                return string.Empty;

            return "WHERE " + string.Join(" AND ", _conditions);
        }

        public SQLiteParameter[] Parameters => _parameters.ToArray();
    }
}
