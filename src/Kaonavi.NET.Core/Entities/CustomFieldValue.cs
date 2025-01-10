namespace Kaonavi.Net.Entities;

/// <summary>基本情報のカスタム項目/シート情報の項目</summary>
public record CustomFieldValue
{
    /// <summary>
    /// 単一の項目値を持つ、CustomFieldValueの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
    /// <param name="value"><inheritdoc cref="Value" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    public CustomFieldValue(int id, string value, string? name = null)
        => (Id, Value, Name) = (id, value, name);

    /// <summary>
    /// 複数の項目値を持つ、CustomFieldValueの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
    /// <param name="values"><inheritdoc cref="Values" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    [JsonConstructor]
    public CustomFieldValue(int id, IReadOnlyList<string> values, string? name = null)
        => (Id, Values, Name) = (id, values, name);

    /// <summary><inheritdoc cref="CustomFieldLayout" path="/param[@name='Id']"/></summary>
    public int Id { get; init; }

    /// <summary>シート項目名</summary>
    public string? Name { get; init; }

    private string? _value;
    /// <summary>シート項目値</summary>
    [JsonIgnore]
    public string Value
    {
        get => _value ??= _values![0];
        init => _value = value;
    }

    private IReadOnlyList<string>? _values;
    /// <summary>シート項目値のリスト</summary>
    /// <remarks>チェックボックスの場合にのみ複数の値が返却されます。</remarks>
    public IReadOnlyList<string> Values
    {
        get => _values ??= [_value!];
        init => _values = value;
    }
}
