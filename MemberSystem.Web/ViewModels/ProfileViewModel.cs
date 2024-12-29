namespace MemberSystem.Web.ViewModels
{
    public class ProfileViewModel
    {
        public int MemberId { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "密碼是必填的")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "確認密碼是必填的")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不一致")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "手機號碼是必填的")]
        [Phone(ErrorMessage = "請輸入有效的電話號碼")]
        public string PhoneNumber { get; set; }

        public string BloodType { get; set; }
    }
}
