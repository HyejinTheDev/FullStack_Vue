using System.ComponentModel.DataAnnotations;
using PaymentService.API.Enums;

namespace PaymentService.API.Entities;

public class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Student;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
