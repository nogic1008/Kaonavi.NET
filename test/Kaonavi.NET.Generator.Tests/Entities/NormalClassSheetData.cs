namespace Kaonavi.Net.Generator.Tests.Entities;

[SheetSerializable]
internal partial class NormalClassSheetData : ISheetData
{
    public string Code { get; set; } = default!;
    [CustomField(101)] public string? Name { get; set; }
    [CustomField(102)] public DateTime Date1 { get; set; }
    [CustomField(103)] public DateTimeOffset Date2 { get; set; }
    [CustomField(104)] public DateOnly Date3 { get; set; }
    [CustomField(105)] public DateTime? Date4 { get; set; }
    [CustomField(106)] public DateTimeOffset? Date5 { get; set; }
    [CustomField(107)] public DateOnly? Date6 { get; set; }
}
