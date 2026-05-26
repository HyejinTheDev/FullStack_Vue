namespace PaymentService.API.DTOs.Reports;

public class RevenueReportDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int PendingInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public List<MonthlyRevenueItem> MonthlyRevenue { get; set; } = new();
    public List<CourseRevenueItem> CourseRevenue { get; set; } = new();
}

public class MonthlyRevenueItem
{
    public string Month { get; set; } = string.Empty; // "2026-03"
    public decimal Revenue { get; set; }
    public int InvoiceCount { get; set; }
}

public class CourseRevenueItem
{
    public string CourseId { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int StudentCount { get; set; }
}
