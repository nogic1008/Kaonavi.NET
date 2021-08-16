namespace ConsoleAppSample;

using System;
using System.Linq;
using Kaonavi.Net.Entities;

/// <summary>
/// 社員情報
/// </summary>
/// <param name="Id">社員番号</param>
/// <param name="Name">名前</param>
/// <param name="NameKana">フリガナ</param>
/// <param name="DepartmentCode">部署コード</param>
/// <param name="MailAddress">メールアドレス</param>
/// <param name="Gender">性別</param>
/// <param name="Birthday">誕生日</param>
/// <param name="BloodType">血液型</param>
/// <param name="EnteredDate">入社日</param>
/// <param name="RetiredDate">退社日</param>
public record EmployeeData(
    string Id,
    string Name,
    string NameKana,
    string DepartmentCode,
    string MailAddress,
    string Gender,
    DateTime Birthday,
    string BloodType,
    DateTime EnteredDate,
    DateTime? RetiredDate = default
)
{
    /// <summary>
    /// カオナビのメンバー情報より、EmployeeDataのインスタンスを生成します。
    /// </summary>
    /// <param name="memberData">メンバー情報</param>
    public EmployeeData(MemberData memberData) : this(
        memberData.Code,
        memberData.Name ?? "",
        memberData.NameKana ?? "",
        memberData.Department?.Code ?? "",
        memberData.Mail ?? "",
        memberData.Gender ?? "",
        memberData.Birthday.GetValueOrDefault(),
        memberData.CustomFields?.FirstOrDefault(c => c.Id == 101)?.Values[0] ?? "",
        memberData.EnteredDate.GetValueOrDefault(),
        memberData.RetiredDate
    )
    { }

    /// <summary>
    /// このインスタンスと等価な<see cref="MemberData"/>を生成します。
    /// </summary>
    public MemberData ToMemberData()
        => new(
            Id,
            Name,
            NameKana,
            MailAddress,
            EnteredDate,
            RetiredDate,
            Gender,
            Birthday,
            new(DepartmentCode),
            CustomFields: new CustomFieldValue[] { new(101, BloodType) }
        );
}
