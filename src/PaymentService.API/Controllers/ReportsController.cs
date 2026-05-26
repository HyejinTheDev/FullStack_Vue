using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.API.Services.Interfaces;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// GET /api/v1/reports/revenue - (Admin) Báo cáo doanh thu
    /// </summary>
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueReport()
    {
        var result = await _reportService.GetRevenueReportAsync();
        return StatusCode(result.StatusCode, result);
    }
}
