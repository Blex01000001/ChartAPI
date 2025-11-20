namespace ChartAPI.Models.Filters
{
    public enum FilterFieldType
    {
        Equal,          // Name = @p
        Like,           // Name LIKE @p
        In,             // Name IN (...)
        Range,          // DateFrom, DateTo
        GreaterEqual,   // >=
        LessEqual,      // <=
        Custom          // QueryBuilder 自訂處理
    }
}
