namespace MemberSystem.Web.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "帳號")]
        [Required(ErrorMessage = "帳號為必填")]
        public string UserName { get; set; }

        [Display(Name = "密碼")]
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; }

        /// <summary>
        /// 登入後導向(框架處理，該欄位通常會藏起來)
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
