using System.Linq.Expressions;

namespace ChartAPI.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<IGrouping<object, T>> GroupByProperty<T>(this IEnumerable<T> source, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Property name cannot be null or empty", nameof(propertyName));

            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.PropertyOrField(param, propertyName);

            // 統一轉成 object，避免型別不一樣時出錯
            var converted = Expression.Convert(body, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(converted, param).Compile();

            return source.GroupBy(lambda);
        }
    }
}
