using System.ComponentModel.DataAnnotations;

namespace ThaiThanhNhan_2287700059_CuoiKi.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
    [Display(Name = "Danh mục")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
