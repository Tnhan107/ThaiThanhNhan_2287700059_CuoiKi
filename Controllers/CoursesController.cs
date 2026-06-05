using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThaiThanhNhan_2287700059_CuoiKi.Services;

namespace ThaiThanhNhan_2287700059_CuoiKi.Controllers;

[Route("courses")]
public class CoursesController : Controller
{
    private const int PageSize = 5;
    private readonly CourseCatalogService _courseCatalogService;
    private readonly UserManager<IdentityUser> _userManager;

    public CoursesController(CourseCatalogService courseCatalogService, UserManager<IdentityUser> userManager)
    {
        _courseCatalogService = courseCatalogService;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var userId = _userManager.GetUserId(User);
        var model = await _courseCatalogService.GetCoursesAsync(search, page, PageSize, userId);
        return View(model);
    }
}
