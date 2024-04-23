namespace Kaonavi.Net.Entities;

/// <summary>拡張アクセス設定の種別</summary>
public enum AdvancedType
{
    /// <summary>社員</summary>
    Member,
    /// <summary>部署</summary>
    Department,
}

/// <summary>拡張アクセス設定</summary>
/// <param name="UserId">ユーザーID</param>
/// <param name="AddCodes">閲覧追加の社員番号、もしくは所属コード</param>
/// <param name="ExclusionCodes">閲覧除外の社員番号、もしくは所属コード</param>
public record AdvancedPermission(
    int UserId,
    IReadOnlyList<string> AddCodes,
    IReadOnlyList<string> ExclusionCodes
);
