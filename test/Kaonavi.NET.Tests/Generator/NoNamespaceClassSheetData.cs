using Kaonavi.Net;

[SheetSerializable]
internal partial class NoNamespaceClassSheetData(string code, string name, DateTime date1, DateTimeOffset date2, DateOnly date3) : ISheetData
{
    public string Code { get; } = code;
    [CustomField(101)] public string Name { get; } = name;
    [CustomField(102)] public DateTime Date1 { get; } = date1;
    [CustomField(103)] public DateTimeOffset Date2 { get; } = date2;
    [CustomField(104)] public DateOnly Date3 { get; } = date3;
    [CustomField(105)] public DateTime? Date4 { get; } = date1;
    [CustomField(106)] public DateTimeOffset? Date5 { get; } = date2;
    [CustomField(107)] public DateOnly? Date6 { get; } = date3;
}
