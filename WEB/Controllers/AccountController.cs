using System.Security.Claims;
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
        public IActionResult Profile()
        {
            ProfileViewModel model = new()
            {
                Email = User.FindFirstValue(ClaimTypes.Email) ?? "",
                FirstName = User.FindFirstValue("FirstName") ?? "",
                LastName = User.FindFirstValue("LastName") ?? "",
                Role = User.FindFirstValue(ClaimTypes.Role) ?? ""
            };

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
    }
}