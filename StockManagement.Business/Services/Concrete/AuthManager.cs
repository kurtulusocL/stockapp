using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StockManagement.Business.Constants.Services;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.DataAccess.Concrete.Context.Azure;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.AuthDtos;
using StockManagement.Shared.Dtos.AuthDtos.OAuthDtos;
using StockManagement.Shared.Helpers;
using StockManagement.Shared.ViewModels.RoleVM;

namespace StockManagement.Business.Services.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        readonly IUserSessionRepository _userSessionRepository;
        readonly IMailService _mailService;
        readonly ITokenService _tokenService;
        readonly IMemoryCache _memoryCache;
        readonly AzureDbContext _azureDbContext;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IOutboxEventRepository _outboxEventRepository;
        public AuthManager(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IUserSessionRepository userSessionRepository, IMailService mailService, ITokenService tokenService, IMemoryCache memoryCache, AzureDbContext azureDbContext, IHttpContextAccessor httpContextAccessor, IOutboxEventRepository outboxEventRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userSessionRepository = userSessionRepository;
            _mailService = mailService;
            _tokenService = tokenService;
            _memoryCache = memoryCache;
            _azureDbContext = azureDbContext;
            _httpContextAccessor = httpContextAccessor;
            _outboxEventRepository = outboxEventRepository;

        }
        public async Task<string?> LoginAsync(LoginDto login)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user == null) return null;
                if (!user.IsActive || user.IsDeleted) return null;

                var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
                if (!result.Succeeded) return null;

                var token = await _tokenService.CreateToken(user);

                var userSession = new UserSession
                {
                    AppUserId = user.Id,
                    Username = user.UserName ?? user.Email,
                    LoginDate = DateTime.Now,
                    IsOnline = true
                };
                await _userSessionRepository.AddAsync(userSession);

                return token;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> GoogleLoginAsync(GoogleLoginDto dto)
        {
            try
            {
                var existingUser = await _userManager.FindByLoginAsync("Google", dto.ProviderKey);
                if (existingUser == null)
                {
                    existingUser = await _userManager.FindByEmailAsync(dto.Email);
                    if (existingUser == null)
                    {
                        var newUser = new AppUser
                        {
                            UserName = dto.Email,
                            Email = dto.Email,
                            NameSurname = dto.NameSurname ?? dto.Email.Split('@')[0],
                            PhoneNumber = string.IsNullOrEmpty(dto.PhoneNumber) ? "-" : dto.PhoneNumber,
                            Title = dto.Title ?? "Google",
                            EmailConfirmed = true,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now,
                            IsLoginConfirmCodeActive = false,
                            ConfirmCode = null
                        };

                        var createResult = await _userManager.CreateAsync(newUser);
                        if (!createResult.Succeeded)
                            return null;

                        var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
                        if (!roleResult.Succeeded)
                            return null;

                        existingUser = newUser;
                    }
                    var loginInfo = new UserLoginInfo("Google", dto.ProviderKey, "Google");
                    var addResult = await _userManager.AddLoginAsync(existingUser, loginInfo);
                    if (addResult.Succeeded)
                    {
                        var loginPayload = new
                        {
                            LoginProvider = "Google",
                            ProviderKey = dto.ProviderKey,
                            ProviderDisplayName = "Google",
                            UserId = existingUser.Id
                        };

                        var outboxEvent = new OutboxEvent
                        {
                            EntityType = "AspNetUserLogins",
                            EventType = "Insert",
                            Payload = JsonConvert.SerializeObject(loginPayload),
                            IsProcessed = false,
                            CreatedDate = DateTime.Now
                        };
                        await _outboxEventRepository.AddAsync(outboxEvent);
                        var userRole = await _roleManager.FindByNameAsync("User");
                        if (userRole != null)
                        {
                            var rolePayload = new
                            {
                                UserId = existingUser.Id,
                                RoleId = userRole.Id
                            };

                            var roleOutboxEvent = new OutboxEvent
                            {
                                EntityType = "AspNetUserRoles",
                                EventType = "Insert",
                                Payload = JsonConvert.SerializeObject(rolePayload),
                                IsProcessed = false,
                                CreatedDate = DateTime.Now
                            };
                            await _outboxEventRepository.AddAsync(roleOutboxEvent);
                        }
                    }
                }
                else
                {
                    if (existingUser.IsDeleted || !existingUser.IsActive)
                        return null;
                }
                var token = await _tokenService.CreateToken(existingUser);

                var userSession = new UserSession
                {
                    AppUserId = existingUser.Id,
                    Username = existingUser.UserName,
                    LoginDate = DateTime.Now,
                    IsOnline = true,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                };
                await _userSessionRepository.AddAsync(userSession);
                return token;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> LoginWithConfirmCodeAsync(LoginDto login)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user == null) throw new ArgumentNullException(nameof(user), "User was null");
                if (!user.IsActive || user.IsDeleted) return null;

                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: true);
                if (!signInResult.Succeeded) return null;

                if (user.IsLoginConfirmCodeActive)
                {
                    int code = new Random().Next(100000, 1000000);
                    _memoryCache.Set($"confirmCode_{user.Email}", code.ToString(), TimeSpan.FromMinutes(15));
                    _memoryCache.Set($"confirmCodeTime_{user.Email}", DateTime.Now, TimeSpan.FromMinutes(15));

                    string mailBody = $"Giriş onay kodunuz: {code}. This code is valid for 5 minutes. Do not share this code with anyone.";
                    await _mailService.SendEmailAsync(user.Email, "System Access Confirmation Code", mailBody);

                    return "confirm_required";
                }

                var token = await _tokenService.CreateToken(user);

                await _userSessionRepository.AddAsync(new UserSession
                {
                    AppUserId = user.Id,
                    Username = user.UserName ?? user.Email,
                    LoginDate = DateTime.Now,
                    IsOnline = true
                });

                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<string?> VerifyLoginConfirmCodeAsync(LoginConfirmCodeDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                    throw new ArgumentNullException(nameof(model.Email), "Email was null");

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User was null");

                var storedCode = _memoryCache.Get<string>($"confirmCode_{model.Email}");
                var creationTime = _memoryCache.Get<DateTime>($"confirmCodeTime_{model.Email}");

                if (storedCode == null)
                    throw new Exception("Confirm code not found. Please request a new code.");

                if ((DateTime.Now - creationTime).TotalSeconds > 300)
                {
                    _memoryCache.Remove($"confirmCode_{model.Email}");
                    _memoryCache.Remove($"confirmCodeTime_{model.Email}");
                    throw new Exception("Code has expired. Please request a new code.");
                }

                if (storedCode != model.LoginConfirmCode.ToString())
                    throw new Exception("Confirm codes are not same.");

                _memoryCache.Remove($"confirmCode_{model.Email}");
                _memoryCache.Remove($"confirmCodeTime_{model.Email}");

                var token = await _tokenService.CreateToken(user);

                await _userSessionRepository.AddAsync(new UserSession
                {
                    AppUserId = user.Id,
                    Username = user.UserName ?? user.Email,
                    LoginDate = DateTime.Now,
                    IsOnline = true
                });
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> ConfirmMailAsync(ConfirmCodeDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                    throw new ArgumentNullException(nameof(model.Email), "Email was null");

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User was null");

                if (user.ConfirmCode != model.ConfirmCode)
                    throw new Exception("Confirm codes are not same.");

                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while confirming email.", ex);
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Model was null");

                var nameSurnameParts = model.NameSurname.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (nameSurnameParts.Length < 2)
                {
                    throw new Exception("Please type to your full Name (for exmaple: Name Surname).");
                }

                var firstName = ConvertTurkishCharacterHelper.ConvertTurkishCharacter(nameSurnameParts[0]);
                var lastName = ConvertTurkishCharacterHelper.ConvertTurkishCharacter(nameSurnameParts[nameSurnameParts.Length - 1]);
                var baseUsername = $"{firstName}{lastName}";
                var username = $"_{baseUsername}_";

                int suffix = 1;
                while (await _userManager.FindByNameAsync(username) != null)
                {
                    username = $"_{baseUsername}{suffix}_";
                    suffix++;
                }

                var user = new AppUser
                {
                    NameSurname = model.NameSurname,
                    UserName = username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Title = model.Title,
                    IsLoginConfirmCodeActive = false,
                    CreatedDate = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                //await _userManager.AddToRoleAsync(user, "rolename");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding new company user.", ex);
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto model, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new ApplicationException($"Unable to load user with ID '{userId}'.");

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                    throw new Exception("There is not a current password");

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded) return false;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while changing password.", ex);
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordDto model, string code)
        {
            try
            {
                if (code == null)
                    throw new ArgumentNullException(nameof(code), "code was null");

                if (model == null)
                    throw new ArgumentNullException(nameof(model), "model was null");

                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    return false;
                }

                AppUser user = new AppUser();
                user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "user was null");
                }
                else
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while reset password.", ex);
            }
        }

        public async Task<UpdateProfileDto> GetDataUpdateProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ArgumentNullException(nameof(user));

            return new UpdateProfileDto
            {
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                IsLoginConfirmCodeActive = user.IsLoginConfirmCodeActive
            };
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfileDto model, string userId)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Model was null");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User was null");

                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.IsLoginConfirmCodeActive = model.IsLoginConfirmCodeActive;
                user.UpdatedDate = DateTime.Now.ToLocalTime();

                IdentityResult result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating profile.", ex);
            }
        }

        public async Task<List<RoleAssignVM>> GetRoleAssignAsync(string id)
        {
            try
            {
                if (id == null)
                    throw new ArgumentNullException(nameof(id), "Id was null");

                AppUser user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User was null");

                List<AppRole> allRoles = _roleManager.Roles.ToList();
                List<string> userRoles = await _userManager.GetRolesAsync(user) as List<string>;

                List<RoleAssignVM> assignRoles = allRoles.Select(role => new RoleAssignVM
                {
                    HasAssign = userRoles.Contains(role.Name),
                    RoleId = role.Id,
                    RoleName = role.Name
                }).ToList();

                return assignRoles;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> RoleAssignAsync(List<RoleAssignVM> modelList, string id)
        {
            try
            {
                if (modelList == null || !modelList.Any())
                    throw new ArgumentNullException(nameof(modelList), "Model list was null or empty");

                if (id == null)
                    throw new ArgumentNullException(nameof(id), "Id was null");

                AppUser user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User was null");

                foreach (RoleAssignVM role in modelList)
                {
                    if (role.HasAssign)
                    {
                        await _userManager.AddToRoleAsync(user, role.RoleName);

                        var existsInAzure = await _azureDbContext.Set<IdentityUserRole<string>>()
                            .FindAsync(id, role.RoleId);
                        if (existsInAzure == null)
                        {
                            await _azureDbContext.Set<IdentityUserRole<string>>()
                                .AddAsync(new IdentityUserRole<string> { UserId = id, RoleId = role.RoleId });
                        }
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.RoleName);

                        var userRole = await _azureDbContext.Set<IdentityUserRole<string>>()
                            .FindAsync(id, role.RoleId);
                        if (userRole != null)
                            _azureDbContext.Set<IdentityUserRole<string>>().Remove(userRole);
                    }
                }
                await _azureDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting role.", ex);
            }
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            try
            {
                userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var activeSessionData = await _userSessionRepository.GetAsync(i => i.AppUserId == userId && i.IsActive && !i.IsDeleted && i.IsOnline == true);
                if (userId != null && activeSessionData != null)
                {
                    activeSessionData.LogoutDate = DateTime.Now.ToLocalTime();
                    var duration = activeSessionData.LogoutDate.Value - activeSessionData.LoginDate;
                    activeSessionData.OnlineDurationSeconds = (int)Math.Round(duration.TotalSeconds);
                    activeSessionData.IsOnline = false;
                    await _userSessionRepository.UpdateAsync(activeSessionData);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
