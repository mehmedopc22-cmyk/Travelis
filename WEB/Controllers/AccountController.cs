using System.Security.Claims;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEB.Helpers;
using WEB.Models;

namespace WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Profile));
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string apiBaseUrl = _configuration["DefaultApiUrl"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                ModelState.AddModelError(string.Empty, "API base URL is not configured.");
                return View(model);
            }

            try
            {
                LoginRequestViewModel request = new()
                {
                    Email = model.Email,
                    Password = model.Password
                };

                LoginResponseViewModel? response = await Utils.CallApiAsync<LoginResponseViewModel>(
                    $"{apiBaseUrl.TrimEnd('/')}/login",
                    HttpMethod.Post,
                    request,
                    cancellationToken: cancellationToken);

                if (response == null || string.IsNullOrWhiteSpace(response.Token))
                {
                    ModelState.AddModelError(string.Empty, "Login failed.");
                    return View(model);
                }

                HttpContext.Session.SetString("JwtToken", response.Token);

                List<Claim> claims =
                [
                    new Claim(ClaimTypes.NameIdentifier, response.UserId.ToString()),
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim("AccessToken", response.Token),
                    new Claim("FirstName", response.FirstName ?? string.Empty),
                    new Claim("LastName", response.LastName ?? string.Empty)
                ];

                if (!string.IsNullOrWhiteSpace(response.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, response.Role));
                }

                ClaimsIdentity identity = new(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                ClaimsPrincipal principal = new(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return RedirectToAction(nameof(Profile));
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("401"))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Login failed: {ex.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Profile));
            }

            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string apiBaseUrl = _configuration["DefaultApiUrl"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                ModelState.AddModelError(string.Empty, "API base URL is not configured.");
                return View(model);
            }

            try
            {
                await Utils.CallApiAsync<string>(
                    $"{apiBaseUrl.TrimEnd('/')}/forgot-password",
                    HttpMethod.Post,
                    model,
                    cancellationToken: cancellationToken);

                TempData["ForgotPasswordMessage"] = "Ако има профил с този имейл, ще получиш временна парола.";
                return RedirectToAction(nameof(Login));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Password reset failed: {ex.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Profile));
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string apiBaseUrl = _configuration["DefaultApiUrl"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                ModelState.AddModelError(string.Empty, "API base URL is not configured.");
                return View(model);
            }

            try
            {
                RegisterRequestViewModel request = new()
                {
                    Email = model.Email,
                    PasswordHash = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,

                    // Temporary placeholder if your API still requires RoleId from client.
                    // Replace with the real default User role Guid OR remove this entirely
                    // once API assigns the default role itself.
                    RoleId = Guid.Empty
                };

                await Utils.CallApiAsync<string>(
                    $"{apiBaseUrl.TrimEnd('/')}/register",
                    HttpMethod.Post,
                    request,
                    cancellationToken: cancellationToken);

                return RedirectToAction(nameof(Login));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Registration failed: {ex.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
                return View(model);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            ProfileViewModel model = BuildProfileViewModel();
            await PopulateProfileImageAsync(model, cancellationToken);

            return View(model);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Profile", model);

            // TODO: call API when you implement update endpoint

            // Update cookie claims (important!)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ""),
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim("FirstName", model.FirstName),
                new Claim("LastName", model.LastName),
                new Claim("AccessToken", User.FindFirstValue("AccessToken") ?? ""),
                new Claim(ClaimTypes.Role, User.FindFirstValue(ClaimTypes.Role) ?? "")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(IFormFile profileImage, CancellationToken cancellationToken)
        {
            ProfileViewModel profileModel = BuildProfileViewModel();

            if (profileImage == null || profileImage.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Избери изображение за профила.");
                return View("Profile", profileModel);
            }

            try
            {
                string imageName = await SaveUploadedImageAsync(profileImage, "profiles", cancellationToken);

                await Utils.CallApiAsync<ImageEntity>(
                    $"{(_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/')}/user/{profileModel.UserId}/profile-image",
                    HttpMethod.Post,
                    HttpContext,
                    new ImageUploadRequestDTO
                    {
                        ImageName = imageName
                    },
                    cancellationToken: cancellationToken);

                TempData["ProfileImageMessage"] = "Профилната снимка беше обновена.";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Profile", profileModel);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, CancellationToken cancellationToken)
        {
            ProfileViewModel profileModel = BuildProfileViewModel();

            if (!ModelState.IsValid)
            {
                return View("Profile", profileModel);
            }

            string apiBaseUrl = _configuration["DefaultApiUrl"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                ModelState.AddModelError(string.Empty, "API base URL is not configured.");
                return View("Profile", profileModel);
            }

            try
            {
                await Utils.CallApiAsync<string>(
                    $"{apiBaseUrl.TrimEnd('/')}/change-password",
                    HttpMethod.Post,
                    HttpContext,
                    model,
                    cancellationToken: cancellationToken);

                TempData["PasswordMessage"] = "Паролата е сменена успешно.";
                return RedirectToAction(nameof(Profile));
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("400"))
            {
                ModelState.AddModelError(string.Empty, "Текущата парола е неправилна.");
                return View("Profile", profileModel);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Password change failed: {ex.Message}");
                return View("Profile", profileModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
                return View("Profile", profileModel);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private ProfileViewModel BuildProfileViewModel()
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);

            return new ProfileViewModel
            {
                UserId = userId,
                Email = User.FindFirstValue(ClaimTypes.Email) ?? "",
                FirstName = User.FindFirstValue("FirstName") ?? "",
                LastName = User.FindFirstValue("LastName") ?? "",
                Role = User.FindFirstValue(ClaimTypes.Role) ?? ""
            };
        }

        private async Task PopulateProfileImageAsync(ProfileViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                ImageEntity? image = await Utils.CallApiAsync<ImageEntity>(
                    $"{(_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/')}/user/{model.UserId}/profile-image",
                    HttpMethod.Get,
                    HttpContext,
                    cancellationToken: cancellationToken);

                model.ProfileImageUrl = BuildUploadUrl("profiles", image?.Name);
            }
            catch (HttpRequestException)
            {
                model.ProfileImageUrl = string.Empty;
            }
        }

        private async Task<string> SaveUploadedImageAsync(IFormFile image, string subfolder, CancellationToken cancellationToken)
        {
            string extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Allowed image formats are JPG, PNG and WEBP.");
            }

            string relativeDirectory = Path.Combine("uploads", subfolder);
            string absoluteDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeDirectory);
            Directory.CreateDirectory(absoluteDirectory);

            string fileName = $"{Guid.NewGuid():N}{extension}";
            string absolutePath = Path.Combine(absoluteDirectory, fileName);

            await using FileStream stream = System.IO.File.Create(absolutePath);
            await image.CopyToAsync(stream, cancellationToken);

            return fileName;
        }

        private static string BuildUploadUrl(string subfolder, string? imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return string.Empty;
            }

            if (imageName.StartsWith("/", StringComparison.Ordinal) || imageName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return imageName;
            }

            return "/" + Path.Combine("uploads", subfolder, imageName).Replace("\\", "/");
        }
    }
}
