using MemberSystem.ApplicationCore.Dtos;
using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Enums;
using MemberSystem.ApplicationCore.Interfaces;
using MemberSystem.ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace MemberSystem.ApplicationCore.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly ITransaction _transaction;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AccountService> _logger;
        //private readonly IPermissionService _permissionService;

        public AccountService(IRepository<Member> memberRepository,
                           ITransaction transaction,
                           IHttpContextAccessor httpContextAccessor,
                           ILogger<AccountService> logger
            //,IPermissionService permissionService
            )
        {
            _memberRepository = memberRepository;
            _transaction = transaction;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            //_permissionService = permissionService;
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task RegisterUserAsync(RegisterDto model)
        {
            try
            {
                _logger.LogInformation("開始進行使用者註冊：{Username}", model.UserName);

                await _transaction.BeginTransactionAsync();

                if (await _memberRepository.AnyAsync(m => m.Username == model.UserName || m.Email == model.Email))
                {
                    _logger.LogError("帳號或信箱已被註冊");
                    throw new InvalidOperationException("帳號或信箱已被註冊");
                }

                var passwordHasher = new PasswordHasher<Member>();

                var memberEntity = new Member
                {
                    Username = model.UserName,
                    Password = passwordHasher.HashPassword(null, model.Password),
                    Email = model.Email,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    BloodType = model.BloodType,
                    IsApproved = model.IsApproved,
                    CreatedAt = DateTime.UtcNow,
                    RoleId = 2, //預設註冊時皆設置為一般註冊會員(User)
                };

                await _memberRepository.AddAsync(memberEntity);
                await _transaction.CommitAsync();
                _logger.LogInformation("使用者註冊成功：{Username}", model.UserName);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                throw; // 拋出邏輯錯誤例外
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                _logger.LogError(ex, "註冊使用者時發生錯誤：{Username}", model.UserName);
                throw;
            }
        }

        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<(ELoginResult Result, LoginDto LoginInfo)> ValidateUserAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return (ELoginResult.UserNotFound, null);
            }

            var member = await _memberRepository.FirstOrDefaultAsync(m => m.Username == userName);
            if (member is null || member.IsApproved != true)
            {
                _logger.LogWarning("登入失敗：帳號 {Username} 不存在", userName);
                return (ELoginResult.UserNotFound, null);
            }

            var passwordHasher = new PasswordHasher<Member>();
            var result = passwordHasher.VerifyHashedPassword(member, member.Password, password);

            if (result != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("登入失敗：密碼錯誤，帳號 {Username}", userName);
                return (ELoginResult.PasswordIncorrect, null);
            }

            var loginDto = new LoginDto
            {
                MemberId = member.MemberId,
                Username = member.Username,
                FullName = member.FullName,
                IsApproved = member.IsApproved,
                RoleId = member.RoleId,
            };

            return (ELoginResult.Success, loginDto);
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public async Task LoginUserAsync(LoginDto model, bool isPersistent = true)
        {
            var roleName = (model.RoleId == 1) ? "Admin" : "User";

            //var permissions = await _permissionService.GetPermissionsAsync(model.RoleId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.FullName),
                new Claim(ClaimTypes.NameIdentifier, model.MemberId.ToString()),
                new Claim("IsApproved", model.IsApproved.HasValue && model.IsApproved.Value ? "true" : "false"),
                new Claim(ClaimTypes.Role, roleName),
               // new Claim("Permissions", string.Join(",", permissions))
            };

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                IsPersistent = isPersistent,
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        }

        /// <summary>
        /// 查詢註冊狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<(bool IsFound, bool? IsApproved)> CheckProgressAsync(RegisterDto model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return (IsFound: false, IsApproved: null);
            }

            var member = await _memberRepository.FirstOrDefaultAsync(m =>
                        (m.Email == model.Email) &&
                        (string.IsNullOrEmpty(model.FullName) || m.FullName == model.FullName) &&
                        (model.DateOfBirth == default || m.DateOfBirth == model.DateOfBirth) &&
                        (string.IsNullOrEmpty(model.PhoneNumber) || m.PhoneNumber == model.PhoneNumber) &&
                        (string.IsNullOrEmpty(model.BloodType) || m.BloodType == model.BloodType));
            if (member == null)
            {
                return (IsFound: false, IsApproved: null);
            }
            return (IsFound: true, member.IsApproved);
        }

        public async Task<(bool IsFound, List<Member> Results)> CheckProgressForAdminAsync(RegisterDto model)
        {
            var results = await _memberRepository.ListAsync();

            _logger.LogInformation("總會員數：{count}", results.Count);

            var members = results.Where(m =>
                         (string.IsNullOrEmpty(model.Email) || m.Email == model.Email) &&
                         (string.IsNullOrEmpty(model.FullName) || m.FullName == model.FullName) &&
                         (model.DateOfBirth == default || m.DateOfBirth == model.DateOfBirth) &&
                         (string.IsNullOrEmpty(model.PhoneNumber) || m.PhoneNumber == model.PhoneNumber) &&
                         (string.IsNullOrEmpty(model.BloodType) || m.BloodType == model.BloodType)).ToList();

            _logger.LogInformation("篩選條件：Email: {email}, FullName: {fullName}, DateOfBirth: {dob}, PhoneNumber: {phone}, BloodType: {blood}",
            model.Email, model.FullName, model.DateOfBirth, model.PhoneNumber, model.BloodType);

            _logger.LogInformation("篩選後的結果數：{count}", members.Count);

            if (!members.Any())
            {
                return (IsFound: false, Results: members);
            }

            return (IsFound: true, members);
        }

        public async Task<bool> UpdateProfileAsync(RegisterDto model)
        {
            try
            {
                _logger.LogInformation("開始進行資料變更：{Username}", model.UserName);
                await _transaction.BeginTransactionAsync();
                var member = await _memberRepository.FirstOrDefaultAsync(m => m.MemberId == model.MemberId);
                if (member == null)
                {
                    _logger.LogError("找不到會員資料");
                    return false;
                }

                member.FullName = model.FullName;
                member.Email = model.Email;
                member.PhoneNumber = model.PhoneNumber;

                await _memberRepository.UpdateAsync(member);

                await _transaction.CommitAsync();
                _logger.LogInformation("完成資料變更：{Username}", model.UserName);
                return true;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdatePasswordAsync(int memberId, string password)
        {
            try
            {
                await _transaction.BeginTransactionAsync();
                var member = await _memberRepository.FirstOrDefaultAsync(m => m.MemberId == memberId);
                var passwordHasher = new PasswordHasher<Member>();
                if (member == null)
                {
                    _logger.LogError("找不到會員資料");
                    return false;
                }

                member.Password = passwordHasher.HashPassword(null, password);
                await _memberRepository.UpdateAsync(member);

                await _transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                return false;
            }
        }
    }
}
