using Kaonavi.Net;

namespace ConsoleAppSample;

[SheetSerializable]
internal partial record CustomSheetData(
    string Code,
    [property: CustomField(101)] string ZipCode,
    [property: CustomField(102)] string Address,
    [property: CustomField(103)] DateOnly Updated
) : ISheetData;
