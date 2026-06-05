using System.ComponentModel.DataAnnotations;

namespace ThaiThanhNhan_2287700059_CuoiKi.Models;

public class Course
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên học phần.")]
    [StringLength(150)]
    [Display(Name = "Tên học phần")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Hình ảnh minh họa")]
    public string? Image { get; set; }

    [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10.")]
    [Display(Name = "Số tín chỉ")]
    public int Credits { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập giảng viên.")]
    [StringLength(120)]
    [Display(Name = "Giảng viên")]
    public string Lecturer { get; set; } = string.Empty;

    [Display(Name = "Danh mục")]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
