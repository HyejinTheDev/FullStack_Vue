namespace PaymentService.API.DTOs.Invoices;

public class InvoiceDto
{
    public Guid InvoiceId { get; set; }
    public Guid UserId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? CourseId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class PayInvoiceResponse
{
    public string PaymentUrl { get; set; } = string.Empty; // URL redirect sang cổng thanh toán (mô phỏng)
    public string Message { get; set; } = string.Empty;
}
