using Microsoft.EntityFrameworkCore;
using ThaiThanhNhan_2287700059_CuoiKi.Data;
using ThaiThanhNhan_2287700059_CuoiKi.ViewModels;

namespace ThaiThanhNhan_2287700059_CuoiKi.Services;

public class CourseCatalogService
{
    private readonly ApplicationDbContext _context;

    public CourseCatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CourseListViewModel> GetCoursesAsync(string? search, int page, int pageSize, string? userId)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Max(pageSize, 1);
        search = search?.Trim();

        var query = _context.Courses
            .Include(course => course.Category)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(course => course.Name.Contains(search));
        }

        var totalItems = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));

        if (page > totalPages)
        {
            page = totalPages;
        }

        var courses = await query
            .OrderBy(course => course.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(course => new CourseCardViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Image = course.Image,
                Credits = course.Credits,
                Lecturer = course.Lecturer,
                CategoryName = course.Category == null ? string.Empty : course.Category.Name
            })
            .ToListAsync();

        var enrolledCourseIds = new HashSet<int>();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var enrolledList = await _context.Enrollments
                .Where(enrollment => enrollment.UserId == userId)
                .Select(enrollment => enrollment.CourseId)
                .ToListAsync();
            enrolledCourseIds = enrolledList.ToHashSet();
        }

        foreach (var course in courses)
        {
            course.IsEnrolled = enrolledCourseIds.Contains(course.Id);
        }

        return new CourseListViewModel
        {
            Courses = courses,
            Search = search,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }
}
