namespace Kaonavi.Net.Entities;

/// <summary>主務/兼務情報</summary>
/// <param name="Code"><inheritdoc cref="DepartmentTree" path="/param[@name='Code']"/></param>
/// <param name="Name">親所属を含む全ての所属名を半角スペース区切りで返却</param>
/// <param name="Names">親所属を含む全ての所属名を配列で返却</param>
public record MemberDepartment(
    string Code,
    string? Name = null,
    IReadOnlyList<string>? Names = null
);
