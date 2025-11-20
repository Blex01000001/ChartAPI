using System.Data.SQLite;
using System.Reflection;

namespace ChartAPI.DataAccess.SQLite.Utilities
{
    public class Materializer
    {
        private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _cache
            = new();

        public T Map<T>(SQLiteDataReader reader) where T : new()
        {
            var type = typeof(T);
            var map = GetPropertyMap(type);

            var model = new T();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string col = reader.GetName(i);

                if (!map.TryGetValue(col, out var prop))
                    continue;

                object value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                value = ConvertValue(value, prop.PropertyType);

                prop.SetValue(model, value);
            }

            return model;
        }

        private Dictionary<string, PropertyInfo> GetPropertyMap(Type type)
        {
            if (_cache.TryGetValue(type, out var map))
                return map;

            map = type.GetProperties()
                      .Where(p => p.CanWrite)
                      .ToDictionary(p => p.Name, p => p);

            _cache[type] = map;
            return map;
        }

        private object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return null;

            Type unwrap = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Enum
            if (unwrap.IsEnum)
                return Enum.ToObject(unwrap, value);

            // 一般型別轉換
            if (value.GetType() != unwrap)
                return Convert.ChangeType(value, unwrap);

            return value;
        }
    }
}
