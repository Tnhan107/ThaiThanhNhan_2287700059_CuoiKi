using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ThaiThanhNhan_2287700059_CuoiKi.Models;

namespace ThaiThanhNhan_2287700059_CuoiKi.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>(entity =>
        {
            entity.Property(category => category.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(category => category.Name).IsUnique();
        });

        builder.Entity<Course>(entity =>
        {
            entity.Property(course => course.Name).HasMaxLength(150).IsRequired();
            entity.Property(course => course.Image).HasMaxLength(500);
            entity.Property(course => course.Lecturer).HasMaxLength(120).IsRequired();
            entity.Property(course => course.Credits).IsRequired();

            entity.HasOne(course => course.Category)
                .WithMany(category => category.Courses)
                .HasForeignKey(course => course.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Enrollment>(entity =>
        {
            entity.Property(enrollment => enrollment.UserId).HasMaxLength(450).IsRequired();
            entity.Property(enrollment => enrollment.EnrollDate).IsRequired();
            entity.HasIndex(enrollment => new { enrollment.UserId, enrollment.CourseId }).IsUnique();

            entity.HasOne(enrollment => enrollment.User)
                .WithMany()
                .HasForeignKey(enrollment => enrollment.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(enrollment => enrollment.Course)
                .WithMany(course => course.Enrollments)
                .HasForeignKey(enrollment => enrollment.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
