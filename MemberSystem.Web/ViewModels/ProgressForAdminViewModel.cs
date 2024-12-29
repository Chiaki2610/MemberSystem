namespace MemberSystem.Web.ViewModels
{
    public class ProgressForAdminViewModel
    {
        [StringLength(32, MinimumLength = 8, ErrorMessage = "帳號長度需在8到32字元之間")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessage = "姓名不得超過50字元")]
        public string FullName { get; set; }

        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Phone(ErrorMessage = "請輸入有效的電話號碼")]
        public string PhoneNumber { get; set; }

        public string BloodType { get; set; }
    }
}
