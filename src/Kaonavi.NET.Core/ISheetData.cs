using Kaonavi.Net.Entities;

namespace Kaonavi.Net;

/// <summary>
/// このエンティティが、<see cref="SheetData"/>に変換可能であることを示します。
/// </summary>
public interface ISheetData
{
    /// <inheritdoc cref="SheetData.Code"/>
    public string Code { get; }

    /// <summary>
    /// このエンティティを、等価な<see cref="CustomFieldValue"/>に変換します。
    /// </summary>

    public IReadOnlyList<CustomFieldValue> ToCustomFields();
}

/// <summary>
/// <see cref="ISheetData"/> の拡張メソッドを提供します。
/// </summary>

public static class ISheetDataExtensions
{
    /// <summary>
    /// <see cref="ISheetData"/>の一覧を、等価な<inheritdoc cref="RecordType.Multiple"/>の<see cref="SheetData"/>に変換します。
    /// </summary>
    /// <typeparam name="T"><see cref="ISheetData"/>を列挙可能なインターフェース</typeparam>
    /// <param name="sheets"></param>
    /// <returns></returns>
    public static IReadOnlyList<SheetData> ToMultipleSheetData<T>(this T sheets) where T : IEnumerable<ISheetData>
    {
        var dic = new Dictionary<string, List<IReadOnlyList<CustomFieldValue>>>();
        foreach (var item in sheets)
        {
            if (dic.TryGetValue(item.Code, out var list))
                list.Add(item.ToCustomFields());
            else
                dic.Add(item.Code, [item.ToCustomFields()]);
        }

        return dic.Select(x => new SheetData(x.Key, x.Value)).ToArray();
    }
}
