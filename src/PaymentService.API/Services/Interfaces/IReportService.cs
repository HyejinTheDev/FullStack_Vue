using PaymentService.API.DTOs;
using PaymentService.API.DTOs.Reports;

namespace PaymentService.API.Services.Interfaces;

public interface IReportService
{
    Task<ApiResponse<RevenueReportDto>> GetRevenueReportAsync();
}
