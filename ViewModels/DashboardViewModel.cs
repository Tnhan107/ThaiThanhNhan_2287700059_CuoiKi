namespace ThaiThanhNhan_2287700059_CuoiKi.ViewModels;

public class DashboardViewModel
{
    public int TotalCourses { get; set; }

    public int TotalStudents { get; set; }

    public int TotalEnrollments { get; set; }

    public IReadOnlyList<CategoryStatisticViewModel> CoursesByCategory { get; set; } = Array.Empty<CategoryStatisticViewModel>();
}

public class CategoryStatisticViewModel
{
    public string CategoryName { get; set; } = string.Empty;

    public int CourseCount { get; set; }
}
