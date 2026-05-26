using Microsoft.EntityFrameworkCore;
using PaymentService.API.Data;
using PaymentService.API.DTOs;
using PaymentService.API.DTOs.Reports;
using PaymentService.API.Enums;
using PaymentService.API.Services.Interfaces;

namespace PaymentService.API.Services.Implementations;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<RevenueReportDto>> GetRevenueReportAsync()
    {
        var invoices = await _db.Invoices.ToListAsync();

        var paidInvoices = invoices.Where(i => i.Status == InvoiceStatus.Paid).ToList();

        // Thống kê doanh thu theo tháng
        var monthlyRevenue = paidInvoices
            .GroupBy(i => i.PaidAt?.ToString("yyyy-MM") ?? i.CreatedAt.ToString("yyyy-MM"))
            .Select(g => new MonthlyRevenueItem
            {
                Month = g.Key,
                Revenue = g.Sum(i => i.Amount),
                InvoiceCount = g.Count()
            })
            .OrderBy(m => m.Month)
            .ToList();

        // Thống kê doanh thu theo khóa học
        var courseRevenue = paidInvoices
            .GroupBy(i => new { i.CourseId, i.CourseName })
            .Select(g => new CourseRevenueItem
            {
                CourseId = g.Key.CourseId ?? "N/A",
                CourseName = g.Key.CourseName,
                Revenue = g.Sum(i => i.Amount),
                StudentCount = g.Select(i => i.UserId).Distinct().Count()
            })
            .OrderByDescending(c => c.Revenue)
            .ToList();

        var report = new RevenueReportDto
        {
            TotalRevenue = paidInvoices.Sum(i => i.Amount),
            TotalInvoices = invoices.Count,
            PaidInvoices = paidInvoices.Count,
            PendingInvoices = invoices.Count(i => i.Status == InvoiceStatus.Pending),
            OverdueInvoices = invoices.Count(i => i.Status == InvoiceStatus.Overdue),
            MonthlyRevenue = monthlyRevenue,
            CourseRevenue = courseRevenue
        };

        return ApiResponse<RevenueReportDto>.Ok(report, "Lấy báo cáo doanh thu thành công");
    }
}
