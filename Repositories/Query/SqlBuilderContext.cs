namespace ChartAPI.Repositories.Query
{
    /// <summary>
    /// Builder 的管理器：負責根據 key/value 選擇適合的 ISqlBuilder
    /// </summary>
    public class SqlBuilderContext
    {
        private readonly List<ISqlBuilder> _builders;

        public SqlBuilderContext()
        {
            // 若你真的想完全用 Activator，也可以改成掃描 Assembly。
            // 這裡採用固定列表 + Activator.CreateInstance，兼顧可讀性與彈性。
            var builderTypes = new[]
            {
                typeof(InSqlBuilder),
                typeof(LikeSqlBuilder),
                typeof(BooleanSqlBuilder),
                typeof(EqualSqlBuilder) // 一定要放最後（fallback）
            };

            _builders = builderTypes
                .Select(t => (ISqlBuilder)Activator.CreateInstance(t)!)
                .ToList();
        }

        public SqlBuildResult Build(string key, object value)
        {
            foreach (var builder in _builders)
            {
                if (builder.CanBuild(key, value))
                {
                    return builder.Build(key, value);
                }
            }

            // 理論上不會到這裡（EqualSqlBuilder 總是會接住）
            return new SqlBuildResult();
        }
    }
}
