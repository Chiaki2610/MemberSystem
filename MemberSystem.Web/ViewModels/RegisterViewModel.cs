namespace MemberSystem.Web.ViewModels
{
    public class RegisterViewModel
    {
        public int MemberId { get; set; }

        [Required(ErrorMessage = "帳號是必填的")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "帳號長度需在8到32字元之間")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密碼是必填的")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "確認密碼是必填的")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不一致")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "電子郵件是必填的")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
        public string Email { get; set; }

        [Required(ErrorMessage = "姓名是必填的")]
        [StringLength(50, ErrorMessage = "姓名不得超過50字元")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "生日是必填的")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "手機號碼是必填的")]
        [Phone(ErrorMessage = "請輸入有效的電話號碼")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "請選擇血型")]
        public string BloodType { get; set; }

    }
}
