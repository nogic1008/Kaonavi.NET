using Kaonavi.Net;

[SheetSerializable]
internal partial record NoNamespaceRecordSheetData(
    string Code,
    [property: CustomField(101)] string Name,
    [property: CustomField(102)] DateTime Date1,
    [property: CustomField(103)] DateTimeOffset Date2,
    [property: CustomField(104)] DateOnly Date3,
    [property: CustomField(105)] DateTime? Date4,
    [property: CustomField(106)] DateTimeOffset? Date5,
    [property: CustomField(107)] DateOnly? Date6
) : ISheetData;
