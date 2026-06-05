using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThaiThanhNhan_2287700059_CuoiKi.Data;
using ThaiThanhNhan_2287700059_CuoiKi.Models;
using ThaiThanhNhan_2287700059_CuoiKi.ViewModels;

namespace ThaiThanhNhan_2287700059_CuoiKi.Controllers;

[Authorize(Roles = RoleNames.Student)]
[Route("enroll")]
public class EnrollController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public EnrollController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("create/{courseId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int courseId, string? returnUrl = null)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Challenge();
        }

        var courseExists = await _context.Courses.AnyAsync(course => course.Id == courseId);
        if (!courseExists)
        {
            return NotFound();
        }

        var alreadyEnrolled = await _context.Enrollments
            .AnyAsync(enrollment => enrollment.UserId == userId && enrollment.CourseId == courseId);

        if (!alreadyEnrolled)
        {
            _context.Enrollments.Add(new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrollDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "Đăng ký học phần thành công.";
        }
        else
        {
            TempData["StatusMessage"] = "Bạn đã đăng ký học phần này.";
        }

        return RedirectBack(returnUrl);
    }

    [HttpPost("cancel/{courseId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int courseId, string? returnUrl = null)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Challenge();
        }

        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(item => item.CourseId == courseId && item.UserId == userId);

        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "Đã hủy đăng ký học phần.";
        }

        return RedirectBack(returnUrl);
    }

    [HttpGet("my-courses")]
    public async Task<IActionResult> MyCourses()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Challenge();
        }

        var courses = await _context.Enrollments
            .Where(enrollment => enrollment.UserId == userId)
            .Include(enrollment => enrollment.Course)
            .ThenInclude(course => course!.Category)
            .OrderByDescending(enrollment => enrollment.EnrollDate)
            .AsNoTracking()
            .Select(enrollment => new MyCourseItemViewModel
            {
                CourseId = enrollment.CourseId,
                Name = enrollment.Course == null ? string.Empty : enrollment.Course.Name,
                Image = enrollment.Course == null ? null : enrollment.Course.Image,
                Credits = enrollment.Course == null ? 0 : enrollment.Course.Credits,
                Lecturer = enrollment.Course == null ? string.Empty : enrollment.Course.Lecturer,
                CategoryName = enrollment.Course == null || enrollment.Course.Category == null ? string.Empty : enrollment.Course.Category.Name,
                EnrollDate = enrollment.EnrollDate
            })
            .ToListAsync();

        return View(new MyCoursesViewModel { Courses = courses });
    }

    private IActionResult RedirectBack(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
