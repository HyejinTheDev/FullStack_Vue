using Microsoft.EntityFrameworkCore;
using PaymentService.API.Entities;
using PaymentService.API.Enums;

namespace PaymentService.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint cho Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Quan hệ User -> Invoices (1-N)
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.User)
            .WithMany(u => u.Invoices)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data: Admin mặc định
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = adminId,
            FullName = "Admin Hệ Thống",
            Email = "admin@trungtam.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed data: Một vài Học viên mẫu
        var student1Id = Guid.Parse("00000000-0000-0000-0000-000000000010");
        var student2Id = Guid.Parse("00000000-0000-0000-0000-000000000011");
        var student3Id = Guid.Parse("00000000-0000-0000-0000-000000000012");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = student1Id,
                FullName = "Nguyễn Đình Minh Hiếu",
                Email = "hieu@student.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Hieu@123"),
                Role = UserRole.Student,
                CreatedAt = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = student2Id,
                FullName = "Nguyễn Công Hiệp",
                Email = "hiep@student.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Hiep@123"),
                Role = UserRole.Student,
                CreatedAt = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = student3Id,
                FullName = "Bùi Thế Đạt",
                Email = "dat@student.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Dat@123"),
                Role = UserRole.Student,
                CreatedAt = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed data: Một số hóa đơn mẫu
        modelBuilder.Entity<Invoice>().HasData(
            new Invoice
            {
                InvoiceId = Guid.Parse("00000000-0000-0000-0001-000000000001"),
                UserId = student1Id,
                CourseName = "IELTS 7.0+",
                CourseId = "CRS-001",
                Amount = 5000000m,
                Status = InvoiceStatus.Paid,
                CreatedAt = new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc),
                PaidAt = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc)
            },
            new Invoice
            {
                InvoiceId = Guid.Parse("00000000-0000-0000-0001-000000000002"),
                UserId = student1Id,
                CourseName = "TOEIC 800+",
                CourseId = "CRS-002",
                Amount = 3500000m,
                Status = InvoiceStatus.Pending,
                CreatedAt = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc)
            },
            new Invoice
            {
                InvoiceId = Guid.Parse("00000000-0000-0000-0001-000000000003"),
                UserId = student2Id,
                CourseName = "Giao tiếp cơ bản",
                CourseId = "CRS-003",
                Amount = 2000000m,
                Status = InvoiceStatus.Paid,
                CreatedAt = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                PaidAt = new DateTime(2026, 3, 16, 0, 0, 0, DateTimeKind.Utc)
            },
            new Invoice
            {
                InvoiceId = Guid.Parse("00000000-0000-0000-0001-000000000004"),
                UserId = student2Id,
                CourseName = "IELTS 7.0+",
                CourseId = "CRS-001",
                Amount = 5000000m,
                Status = InvoiceStatus.Overdue,
                CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Invoice
            {
                InvoiceId = Guid.Parse("00000000-0000-0000-0001-000000000005"),
                UserId = student3Id,
                CourseName = "Luyện thi N3 tiếng Nhật",
                CourseId = "CRS-004",
                Amount = 4500000m,
                Status = InvoiceStatus.Pending,
                CreatedAt = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Invoice
            {
                InvoiceId = Guid.Parse("00000000-0000-0000-0001-000000000006"),
                UserId = student3Id,
                CourseName = "TOEIC 800+",
                CourseId = "CRS-002",
                Amount = 3500000m,
                Status = InvoiceStatus.Paid,
                CreatedAt = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                PaidAt = new DateTime(2026, 4, 3, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
