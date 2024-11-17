namespace Kaonavi.Net.Entities;

/// <summary>シート項目種別</summary>
[JsonConverter(typeof(JsonStringEnumConverter<FieldInput>))]
public enum FieldInput
{
    /// <summary>テキストボックス</summary>
    [JsonStringEnumMemberName("text_box")] TextBox,
    /// <summary>テキストエリア</summary>
    [JsonStringEnumMemberName("text_area")] TextArea,
    /// <summary>ナンバーボックス</summary>
    [JsonStringEnumMemberName("number_box")] NumberBox,
    /// <summary>プルダウンリスト</summary>
    [JsonStringEnumMemberName("pull_down")] PullDown,
    /// <summary>ラジオボタン</summary>
    [JsonStringEnumMemberName("radio_button")] RadioButton,
    /// <summary>チェックボックス</summary>
    [JsonStringEnumMemberName("check_box")] CheckBox,
    /// <summary>リンク</summary>
    [JsonStringEnumMemberName("link")] Link,
    /// <summary>日付（カレンダー）</summary>
    [JsonStringEnumMemberName("date")] Date,
    /// <summary>年月（カレンダー）</summary>
    [JsonStringEnumMemberName("year_month")] YearMonth,
    /// <summary>ファイル添付</summary>
    [JsonStringEnumMemberName("attach_file")] AttachFile,
    /// <summary>顔写真</summary>
    [JsonStringEnumMemberName("face_image")] FaceImage,
    /// <summary>計算式</summary>
    [JsonStringEnumMemberName("calc_formula")] CalcFormula,
}
