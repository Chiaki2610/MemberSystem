namespace MemberSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var model = new AccountViewModel
            {
                LoginViewModel = new LoginViewModel
                {
                    ReturnUrl = returnUrl
                },
                RegisterViewModel = new RegisterViewModel(),
                IsAuthenticated = User.Identity.IsAuthenticated // 判斷是否登入
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            var request = model.LoginViewModel;

            if (!ModelState.IsValid)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "登入失敗，請檢查您的輸入資料";
                return View(model);
            }

            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "帳號或密碼不可為空";
                return View(model);
            }

            var member = await _userService.ValidateUserAsync(model.LoginViewModel.UserName, model.LoginViewModel.Password);

            if (member.LoginInfo is null)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "帳號不存在或帳號/密碼錯誤，請重新再操作一次";
                return View(model);
            }

            await _userService.LoginUserAsync(member.LoginInfo);

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "登入成功！";

            if (string.IsNullOrEmpty(request.ReturnUrl))
            {
                request.ReturnUrl = "/";
            }

            return Redirect(request.ReturnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now),

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 利用SweetAlert將錯誤訊息傳遞給使用者
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "註冊資料有誤，請檢查後重新再試一次。";
                return View(model);
            }
            try
            {
                var dto = new RegisterDto
                {
                    UserName = model.UserName.ToLower(),
                    Email = model.Email.ToLower(),
                    Password = model.Password,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    BloodType = model.BloodType,
                    IsApproved = null,
                };

                // 註冊同時將驗證帳號or信箱是否已被註冊，註冊成功才會導轉回首頁
                await _userService.RegisterUserAsync(dto);

                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "註冊申請成功，請等候審核結果；如欲查詢進度請利用「Registration Status」頁面，謝謝。";
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "註冊失敗");
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "發生未預期的錯誤，請稍後重試；如有不便，還請見諒。";
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// 鑒於個資法，設定惟本人得查詢審核，他人不得透過其他欄位查得結果
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Progress()
        {
            var model = new ProgressViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Progress(ProgressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "請檢查輸入的資料是否正確！";
                return View(model);
            }

            try
            {
                var registerDto = new RegisterDto
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    BloodType = model.BloodType,
                };

                var (isFound, isApproved) = await _userService.CheckProgressAsync(registerDto);

                if (!isFound)
                {
                    TempData["ToastType"] = "error";
                    TempData["ToastMessage"] = "查無此筆申請資料或條件有誤，請確認資料後重新再試一次！";
                    return View(model);
                }
                else
                {
                    if (isApproved == null)
                    {
                        TempData["ToastType"] = "error";
                        TempData["ToastMessage"] = "申請尚未進行審核。";
                        return View(model);
                    }
                    TempData["ToastType"] = isApproved == true ? "success" : "warning"; ;
                    TempData["ToastMessage"] = isApproved switch
                    {
                        true => "恭喜！您的申請已通過審核！🎉",
                        false => "很抱歉，您的申請並未通過審核 😞；如有其他需求請聯繫客服。",
                        _ => "申請尚未進行審核。"
                    };
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "查詢時發生錯誤，請稍後再試！";
                _logger.LogError(ex, "查詢審核進度時發生錯誤");
                return View(model);
            }
        }

        /// <summary>
        /// 可以任意條件查看結果的特殊頁面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ProgressForAdmin()
        {
            var model = new ProgressForAdminViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProgressForAdmin(ProgressForAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "請檢查輸入的資料是否正確！";
                return View(model);
            }

            try
            {
                var registerDto = new RegisterDto
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    BloodType = model.BloodType,
                };

                var (isFound, results) = await _userService.CheckProgressForAdminAsync(registerDto);

                if (!isFound)
                {
                    TempData["ToastType"] = "error";
                    TempData["ToastMessage"] = "查無此申請資料，請確認資料後重新再試一次！";
                    return View(model);
                }

                return View("ProgressResults", results);
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "查詢時發生錯誤，請稍後再試！";
                _logger.LogError(ex, "查詢審核進度時發生錯誤");
                return View(model);
            }
        }
    }
}
