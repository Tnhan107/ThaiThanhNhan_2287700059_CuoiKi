namespace ThaiThanhNhan_2287700059_CuoiKi.ViewModels;

public class CourseListViewModel
{
    public IReadOnlyList<CourseCardViewModel> Courses { get; set; } = Array.Empty<CourseCardViewModel>();

    public string? Search { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 5;

    public int TotalItems { get; set; }

    public int TotalPages { get; set; } = 1;

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;
}

public class CourseCardViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Image { get; set; }

    public int Credits { get; set; }

    public string Lecturer { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public bool IsEnrolled { get; set; }
}
