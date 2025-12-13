using SqlKata;
using System.Data.SQLite;

namespace ChartAPI.DataAccess.SQLite.Utilities
{
    public static class SqlKataSqliteHelper
    {
        public static SQLiteParameter[] ToSqliteParameters(
            SqlResult result)
        {
            var parameters = new SQLiteParameter[result.Bindings.Count];

            for (int i = 0; i < result.Bindings.Count; i++)
            {
                parameters[i] = new SQLiteParameter(
                    $"@p{i}",
                    result.Bindings[i] ?? DBNull.Value
                );
            }
            return parameters;
        }
    }
}
