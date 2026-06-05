using Microsoft.AspNetCore.Identity;

namespace ThaiThanhNhan_2287700059_CuoiKi.Models;

public class Enrollment
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public IdentityUser? User { get; set; }

    public int CourseId { get; set; }

    public Course? Course { get; set; }

    public DateTime EnrollDate { get; set; } = DateTime.UtcNow;
}
