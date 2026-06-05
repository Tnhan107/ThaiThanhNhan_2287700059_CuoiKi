using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThaiThanhNhan_2287700059_CuoiKi.Models;

namespace ThaiThanhNhan_2287700059_CuoiKi.Data;

public static class DbInitializer
{
    private static readonly Dictionary<string, string> CourseImages = new()
    {
        ["Lap trinh ASP.NET Core MVC"] = "https://images.unsplash.com/photo-1498050108023-c5249f4df085?auto=format&fit=crop&w=900&q=80",
        ["Co so du lieu voi Entity Framework"] = "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?auto=format&fit=crop&w=900&q=80",
        ["Thiet ke Web nang cao"] = "https://images.unsplash.com/photo-1467232004584-a241de8bcf5d?auto=format&fit=crop&w=900&q=80",
        ["Bao mat ung dung Web"] = "https://images.unsplash.com/photo-1563986768494-4dee2763ff3f?auto=format&fit=crop&w=900&q=80",
        ["Phan tich va thiet ke he thong"] = "https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?auto=format&fit=crop&w=900&q=80",
        ["Quan tri du an phan mem"] = "https://images.unsplash.com/photo-1552664730-d307ca884978?auto=format&fit=crop&w=900&q=80",
        ["Marketing can ban"] = "https://images.unsplash.com/photo-1432888622747-4eb9a8efeb07?auto=format&fit=crop&w=900&q=80",
        ["Khoi nghiep doi moi sang tao"] = "https://images.unsplash.com/photo-1556761175-b413da4baf72?auto=format&fit=crop&w=900&q=80",
        ["Tieng Anh chuyen nganh CNTT"] = "https://images.unsplash.com/photo-1501504905252-473c47e087f8?auto=format&fit=crop&w=900&q=80",
        ["Giao tiep va lam viec nhom"] = "https://images.unsplash.com/photo-1522071820081-009f0129c71c?auto=format&fit=crop&w=900&q=80",
        ["Lap trinh ung dung di dong"] = "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?auto=format&fit=crop&w=900&q=80",
        ["Tri tue nhan tao can ban"] = "https://images.unsplash.com/photo-1677442136019-21780ecad995?auto=format&fit=crop&w=900&q=80"
    };

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        await context.Database.MigrateAsync();

        await EnsureRoleAsync(roleManager, RoleNames.Admin);
        await EnsureRoleAsync(roleManager, RoleNames.Student);

        await EnsureUserAsync(userManager, "admin", "admin@coursereg.local", "Admin@12345", RoleNames.Admin);
        await EnsureUserAsync(userManager, "student", "student@coursereg.local", "Student@12345", RoleNames.Student);

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Cong nghe thong tin" },
                new Category { Name = "Kinh te" },
                new Category { Name = "Ngoai ngu" },
                new Category { Name = "Ky nang mem" });

            await context.SaveChangesAsync();
        }

        if (!await context.Courses.AnyAsync())
        {
            var categories = await context.Categories.ToDictionaryAsync(category => category.Name, category => category.Id);

            context.Courses.AddRange(
                new Course { Name = "Lap trinh ASP.NET Core MVC", Credits = 3, Lecturer = "ThS. Nguyen Van An", CategoryId = categories["Cong nghe thong tin"], Image = CourseImages["Lap trinh ASP.NET Core MVC"] },
                new Course { Name = "Co so du lieu voi Entity Framework", Credits = 3, Lecturer = "ThS. Tran Thi Binh", CategoryId = categories["Cong nghe thong tin"], Image = CourseImages["Co so du lieu voi Entity Framework"] },
                new Course { Name = "Thiet ke Web nang cao", Credits = 3, Lecturer = "ThS. Le Minh Khoa", CategoryId = categories["Cong nghe thong tin"], Image = CourseImages["Thiet ke Web nang cao"] },
                new Course { Name = "Bao mat ung dung Web", Credits = 2, Lecturer = "TS. Pham Hoang Long", CategoryId = categories["Cong nghe thong tin"], Image = CourseImages["Bao mat ung dung Web"] },
                new Course { Name = "Phan tich va thiet ke he thong", Credits = 3, Lecturer = "ThS. Vo Thanh Tam", CategoryId = categories["Cong nghe thong tin"], Image = CourseImages["Phan tich va thiet ke he thong"] },
                new Course { Name = "Quan tri du an phan mem", Credits = 2, Lecturer = "ThS. Huynh Bao Chau", CategoryId = categories["Ky nang mem"], Image = CourseImages["Quan tri du an phan mem"] },
                new Course { Name = "Marketing can ban", Credits = 2, Lecturer = "ThS. Dang Quoc Viet", CategoryId = categories["Kinh te"], Image = CourseImages["Marketing can ban"] },
                new Course { Name = "Khoi nghiep doi moi sang tao", Credits = 2, Lecturer = "TS. Mai Phuong Linh", CategoryId = categories["Kinh te"], Image = CourseImages["Khoi nghiep doi moi sang tao"] },
                new Course { Name = "Tieng Anh chuyen nganh CNTT", Credits = 3, Lecturer = "ThS. Sarah Nguyen", CategoryId = categories["Ngoai ngu"], Image = CourseImages["Tieng Anh chuyen nganh CNTT"] },
                new Course { Name = "Giao tiep va lam viec nhom", Credits = 2, Lecturer = "ThS. Phan Kim Ngan", CategoryId = categories["Ky nang mem"], Image = CourseImages["Giao tiep va lam viec nhom"] },
                new Course { Name = "Lap trinh ung dung di dong", Credits = 3, Lecturer = "ThS. Bui Tuan Anh", CategoryId = categories["Cong nghe thong tin"], Image = CourseImages["Lap trinh ung dung di dong"] },
                new Course { Name = "Tri tue nhan tao can ban", Credits = 3, Lecturer = "TS. Do Anh Thu", CategoryId = categories["Cong nghe thong tin"], Image = CourseImages["Tri tue nhan tao can ban"] });

            await context.SaveChangesAsync();
        }

        await UpdateCourseImagesAsync(context);
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task UpdateCourseImagesAsync(ApplicationDbContext context)
    {
        var courses = await context.Courses.ToListAsync();
        var changed = false;

        foreach (var course in courses)
        {
            var hasOldPlaceholder = course.Image?.Contains("placehold.co", StringComparison.OrdinalIgnoreCase) ?? false;
            if (hasOldPlaceholder && CourseImages.TryGetValue(course.Name, out var image))
            {
                course.Image = image;
                changed = true;
            }
        }

        if (changed)
        {
            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureUserAsync(UserManager<IdentityUser> userManager, string userName, string email, string password, string roleName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not create seed user '{userName}': {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}
