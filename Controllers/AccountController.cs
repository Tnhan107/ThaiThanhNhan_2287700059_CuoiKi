using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThaiThanhNhan_2287700059_CuoiKi.Models;
using ThaiThanhNhan_2287700059_CuoiKi.ViewModels;

namespace ThaiThanhNhan_2287700059_CuoiKi.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    [HttpGet("register")]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new IdentityUser
        {
            UserName = model.UserName.Trim(),
            Email = model.Email.Trim(),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, RoleNames.Student);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [AllowAnonymous]
    [HttpGet("login")]
    public async Task<IActionResult> Login()
    {
        return View(new LoginViewModel
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.UserName.Trim())
            ?? await _userManager.FindByEmailAsync(model.UserName.Trim());

        if (user == null || user.UserName == null)
        {
            ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
        return View(model);
    }

    [Authorize]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpPost("external-login")]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [AllowAnonymous]
    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(string? remoteError = null)
    {
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Google login error: {remoteError}");
            return View("Login", new LoginViewModel
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            });
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (signInResult.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var userName = email ?? $"google_{info.ProviderKey}";
        var user = !string.IsNullOrWhiteSpace(email)
            ? await _userManager.FindByEmailAsync(email)
            : await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Login", new LoginViewModel
                {
                    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                });
            }

            await _userManager.AddToRoleAsync(user, RoleNames.Student);
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        if (!addLoginResult.Succeeded && addLoginResult.Errors.All(error => error.Code != "LoginAlreadyAssociated"))
        {
            foreach (var error in addLoginResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Login", new LoginViewModel
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            });
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }
}
