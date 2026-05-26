using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.API.DTOs.Payments;
using PaymentService.API.DTOs.Invoices;
using PaymentService.API.Services.Interfaces;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/v1")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    /// <summary>
    /// GET /api/v1/invoices - Lấy danh sách hóa đơn
    /// Học viên lấy của mình, Admin lấy tất cả
    /// </summary>
    [Authorize]
    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");
        var result = await _invoiceService.GetInvoicesAsync(userId, isAdmin, pageIndex, pageSize);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// GET /api/v1/invoices/{invoiceId} - Lấy chi tiết hóa đơn
    /// </summary>
    [Authorize]
    [HttpGet("invoices/{invoiceId}")]
    public async Task<IActionResult> GetInvoiceById(Guid invoiceId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");
        var result = await _invoiceService.GetInvoiceByIdAsync(invoiceId, userId, isAdmin);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// POST /api/v1/invoices/{invoiceId}/pay - Tạo yêu cầu thanh toán
    /// </summary>
    [Authorize]
    [HttpPost("invoices/{invoiceId}/pay")]
    public async Task<IActionResult> PayInvoice(Guid invoiceId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");
        var result = await _invoiceService.PayInvoiceAsync(invoiceId, userId, isAdmin);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// POST /api/v1/payments/webhook - Nhận IPN callback từ VNPay/Momo (Public)
    /// </summary>
    [HttpPost("payments/webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> PaymentWebhook([FromBody] PaymentWebhookRequest request)
    {
        var result = await _invoiceService.HandleWebhookAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// POST /api/v1/invoices - Tạo hóa đơn mới (Dành cho Nhóm 7 & 8 gọi sang)
    /// </summary>
    [HttpPost("invoices")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        var result = await _invoiceService.CreateInvoiceAsync(request);
        return StatusCode(result.StatusCode, result);
    }
}
