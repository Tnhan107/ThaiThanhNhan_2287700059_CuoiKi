using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ThaiThanhNhan_2287700059_CuoiKi.ViewModels;

public class CourseFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên học phần.")]
    [StringLength(150)]
    [Display(Name = "Tên học phần")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Đường dẫn hình ảnh")]
    public string? Image { get; set; }

    [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10.")]
    [Display(Name = "Số tín chỉ")]
    public int Credits { get; set; } = 3;

    [Required(ErrorMessage = "Vui lòng nhập giảng viên.")]
    [StringLength(120)]
    [Display(Name = "Giảng viên")]
    public string Lecturer { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục.")]
    [Display(Name = "Danh mục")]
    public int CategoryId { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = Array.Empty<SelectListItem>();
}
