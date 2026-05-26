using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PaymentService.API.Enums;

namespace PaymentService.API.Entities;

public class Invoice
{
    [Key]
    public Guid InvoiceId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required, MaxLength(100)]
    public string CourseName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? CourseId { get; set; } // ID khóa học từ service Nhóm 1

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    // Navigation
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
