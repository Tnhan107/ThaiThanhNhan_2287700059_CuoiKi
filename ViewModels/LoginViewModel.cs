using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace ThaiThanhNhan_2287700059_CuoiKi.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập username hoặc email.")]
    [Display(Name = "Username hoặc Email")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool RememberMe { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
}
