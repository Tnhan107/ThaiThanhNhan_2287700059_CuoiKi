using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThaiThanhNhan_2287700059_CuoiKi.Data;
using ThaiThanhNhan_2287700059_CuoiKi.Models;
using ThaiThanhNhan_2287700059_CuoiKi.ViewModels;

namespace ThaiThanhNhan_2287700059_CuoiKi.Controllers;

[Authorize(Roles = RoleNames.Admin)]
[Route("admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> Index()
    {
        var students = await _userManager.GetUsersInRoleAsync(RoleNames.Student);
        var model = new DashboardViewModel
        {
            TotalCourses = await _context.Courses.CountAsync(),
            TotalStudents = students.Count,
            TotalEnrollments = await _context.Enrollments.CountAsync(),
            CoursesByCategory = await _context.Categories
                .AsNoTracking()
                .Select(category => new CategoryStatisticViewModel
                {
                    CategoryName = category.Name,
                    CourseCount = category.Courses.Count
                })
                .OrderByDescending(statistic => statistic.CourseCount)
                .ToListAsync()
        };

        return View(model);
    }

    [HttpGet("courses")]
    public async Task<IActionResult> Courses()
    {
        var courses = await _context.Courses
            .Include(course => course.Category)
            .OrderBy(course => course.Name)
            .AsNoTracking()
            .ToListAsync();

        return View(courses);
    }

    [HttpGet("courses/create")]
    public async Task<IActionResult> Create()
    {
        var model = new CourseFormViewModel();
        await PopulateCategoriesAsync(model);
        return View(model);
    }

    [HttpPost("courses/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(model);
            return View(model);
        }

        var course = new Course
        {
            Name = model.Name.Trim(),
            Image = string.IsNullOrWhiteSpace(model.Image) ? null : model.Image.Trim(),
            Credits = model.Credits,
            Lecturer = model.Lecturer.Trim(),
            CategoryId = model.CategoryId
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        TempData["StatusMessage"] = "Đã tạo học phần mới.";

        return RedirectToAction(nameof(Courses));
    }

    [HttpGet("courses/edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        var model = new CourseFormViewModel
        {
            Id = course.Id,
            Name = course.Name,
            Image = course.Image,
            Credits = course.Credits,
            Lecturer = course.Lecturer,
            CategoryId = course.CategoryId
        };
        await PopulateCategoriesAsync(model);

        return View(model);
    }

    [HttpPost("courses/edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(model);
            return View(model);
        }

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        course.Name = model.Name.Trim();
        course.Image = string.IsNullOrWhiteSpace(model.Image) ? null : model.Image.Trim();
        course.Credits = model.Credits;
        course.Lecturer = model.Lecturer.Trim();
        course.CategoryId = model.CategoryId;

        await _context.SaveChangesAsync();
        TempData["StatusMessage"] = "Đã cập nhật học phần.";

        return RedirectToAction(nameof(Courses));
    }

    [HttpGet("courses/delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses
            .Include(item => item.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    [HttpPost("courses/delete/{id:int}")]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        TempData["StatusMessage"] = "Đã xóa học phần.";

        return RedirectToAction(nameof(Courses));
    }

    private async Task PopulateCategoriesAsync(CourseFormViewModel model)
    {
        model.Categories = await _context.Categories
            .OrderBy(category => category.Name)
            .Select(category => new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = category.Name,
                Selected = category.Id == model.CategoryId
            })
            .ToListAsync();
    }
}
