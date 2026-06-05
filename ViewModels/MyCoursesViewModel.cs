namespace ThaiThanhNhan_2287700059_CuoiKi.ViewModels;

public class MyCoursesViewModel
{
    public IReadOnlyList<MyCourseItemViewModel> Courses { get; set; } = Array.Empty<MyCourseItemViewModel>();
}

public class MyCourseItemViewModel
{
    public int CourseId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Image { get; set; }

    public int Credits { get; set; }

    public string Lecturer { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public DateTime EnrollDate { get; set; }
}
