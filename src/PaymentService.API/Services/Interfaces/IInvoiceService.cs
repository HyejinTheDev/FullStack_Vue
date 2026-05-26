using PaymentService.API.DTOs;
using PaymentService.API.DTOs.Invoices;
using PaymentService.API.DTOs.Payments;

namespace PaymentService.API.Services.Interfaces;

public interface IInvoiceService
{
    Task<ApiResponse<List<InvoiceDto>>> GetInvoicesAsync(Guid? userId, bool isAdmin, int pageIndex, int pageSize);
    Task<ApiResponse<InvoiceDto>> GetInvoiceByIdAsync(Guid invoiceId, Guid userId, bool isAdmin);
    Task<ApiResponse<PayInvoiceResponse>> PayInvoiceAsync(Guid invoiceId, Guid userId, bool isAdmin);
    Task<ApiResponse<object>> HandleWebhookAsync(PaymentWebhookRequest request);
    Task<ApiResponse<InvoiceDto>> CreateInvoiceAsync(CreateInvoiceRequest request);
}
