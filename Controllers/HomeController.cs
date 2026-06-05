using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThaiThanhNhan_2287700059_CuoiKi.Models;
using ThaiThanhNhan_2287700059_CuoiKi.Services;

namespace ThaiThanhNhan_2287700059_CuoiKi.Controllers;

public class HomeController : Controller
{
    private const int PageSize = 5;
    private readonly CourseCatalogService _courseCatalogService;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(CourseCatalogService courseCatalogService, UserManager<IdentityUser> userManager)
    {
        _courseCatalogService = courseCatalogService;
        _userManager = userManager;
    }

    [HttpGet("/")]
    [HttpGet("/home")]
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var userId = _userManager.GetUserId(User);
        var model = await _courseCatalogService.GetCoursesAsync(search, page, PageSize, userId);
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
