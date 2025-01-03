using MemberSystem.ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MemberSystem.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IAccountService _userService;
        private readonly ProfileViewModelService _profileViewModelService;
        private readonly ILogger<UserController> _logger;

        public UserController(IAccountService userService,
                                ProfileViewModelService profileViewModelService,
                                ILogger<UserController> logger)
        {
            _userService = userService;
            _profileViewModelService = profileViewModelService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var memberId = GetMemberClaim();
            if (memberId < 0) return RedirectToAction("Login", "Account");
            var memberData = await _profileViewModelService.GetMemberData(memberId);

            if (memberData == null)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "找不到會員資料";
                return RedirectToAction("Login", "Account");
            }

            return View(memberData);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(RegisterViewModel data)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "請檢查輸入的資料是否正確！";
                return View(data);
            }
            try
            {
                if (!string.IsNullOrEmpty(data.Password))
                {
                    if (data.Password != data.ConfirmPassword)
                    {
                        TempData["ToastType"] = "error";
                        TempData["ToastMessage"] = "密碼與確認密碼不一致，請重新輸入！";
                        return RedirectToAction("Profile");
                    }
                    await _userService.UpdatePasswordAsync(data.MemberId, data.Password);
                }
                var model = new RegisterDto
                {
                    MemberId = data.MemberId,
                    FullName = data.FullName,
                    Email = data.Email,
                    PhoneNumber = data.PhoneNumber,
                };
                await _userService.UpdateProfileAsync(model);

                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "會員資料更新成功！";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新會員資料時發生錯誤");
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "更新會員資料失敗，請稍後再試！";
                return RedirectToAction("Profile");
            }
        }

        private int GetMemberClaim()
        {
            var memberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (memberIdClaim == null)
            {
                return -1;
            }
            if (!int.TryParse(memberIdClaim.Value, out int memberId))
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "無效的會員";
                return -1;
            }

            return memberId;
        }
    }
}
