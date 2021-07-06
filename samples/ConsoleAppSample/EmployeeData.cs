using System;
using System.Linq;
using Kaonavi.Net.Entities;

namespace ConsoleAppSample
{
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
                customFields: new CustomFieldValue[] { new(101, BloodType) }
            );
    }
}
