using Microsoft.EntityFrameworkCore;
using PaymentService.API.Data;
using PaymentService.API.DTOs;
using PaymentService.API.DTOs.Invoices;
using PaymentService.API.DTOs.Payments;
using PaymentService.API.Enums;
using PaymentService.API.Entities;
using PaymentService.API.Services.Interfaces;

namespace PaymentService.API.Services.Implementations;

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _db;

    public InvoiceService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<List<InvoiceDto>>> GetInvoicesAsync(Guid? userId, bool isAdmin, int pageIndex, int pageSize)
    {
        var query = _db.Invoices.Include(i => i.User).AsQueryable();

        // Nếu không phải Admin thì chỉ lấy invoice của chính mình
        if (!isAdmin && userId.HasValue)
        {
            query = query.Where(i => i.UserId == userId.Value);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var invoices = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = ApiResponse<List<InvoiceDto>>.Ok(
            invoices.Select(i => new InvoiceDto
            {
                InvoiceId = i.InvoiceId,
                UserId = i.UserId,
                StudentName = i.User.FullName,
                CourseName = i.CourseName,
                CourseId = i.CourseId,
                Amount = i.Amount,
                Status = i.Status.ToString(),
                CreatedAt = i.CreatedAt,
                PaidAt = i.PaidAt
            }).ToList(),
            "Lấy danh sách hóa đơn thành công"
        );

        response.Pagination = new PaginationMeta
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = pageIndex < totalPages,
            HasPreviousPage = pageIndex > 1
        };

        return response;
    }

    public async Task<ApiResponse<InvoiceDto>> GetInvoiceByIdAsync(Guid invoiceId, Guid userId, bool isAdmin)
    {
        var invoice = await _db.Invoices.Include(i => i.User).FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        if (invoice == null)
            return ApiResponse<InvoiceDto>.Fail(404, "Không tìm thấy hóa đơn");

        // Kiểm tra quyền: chỉ chủ hóa đơn hoặc Admin mới được xem
        if (!isAdmin && invoice.UserId != userId)
            return ApiResponse<InvoiceDto>.Fail(403, "Bạn không có quyền xem hóa đơn này");

        return ApiResponse<InvoiceDto>.Ok(new InvoiceDto
        {
            InvoiceId = invoice.InvoiceId,
            UserId = invoice.UserId,
            StudentName = invoice.User.FullName,
            CourseName = invoice.CourseName,
            CourseId = invoice.CourseId,
            Amount = invoice.Amount,
            Status = invoice.Status.ToString(),
            CreatedAt = invoice.CreatedAt,
            PaidAt = invoice.PaidAt
        }, "Lấy chi tiết hóa đơn thành công");
    }

    public async Task<ApiResponse<PayInvoiceResponse>> PayInvoiceAsync(Guid invoiceId, Guid userId, bool isAdmin)
    {
        var invoice = await _db.Invoices.FindAsync(invoiceId);
        if (invoice == null)
            return ApiResponse<PayInvoiceResponse>.Fail(404, "Không tìm thấy hóa đơn");

        if (!isAdmin && invoice.UserId != userId)
            return ApiResponse<PayInvoiceResponse>.Fail(403, "Bạn không có quyền thanh toán hóa đơn này");

        if (invoice.Status == InvoiceStatus.Paid)
            return ApiResponse<PayInvoiceResponse>.Fail(400, "Hóa đơn này đã được thanh toán");

        // Mô phỏng: Chuyển trạng thái sang Paid trực tiếp (Thay vì redirect tới cổng thanh toán)
        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse<PayInvoiceResponse>.Ok(new PayInvoiceResponse
        {
            PaymentUrl = $"https://payment.vnpay.vn/mock?invoiceId={invoiceId}",
            Message = "Thanh toán thành công (mô phỏng)"
        }, "Thanh toán hóa đơn thành công");
    }

    public async Task<ApiResponse<object>> HandleWebhookAsync(PaymentWebhookRequest request)
    {
        // Mô phỏng xử lý IPN callback từ VNPay/Momo
        if (!Guid.TryParse(request.InvoiceId, out var invoiceId))
            return ApiResponse<object>.Fail(400, "InvoiceId không hợp lệ");

        var invoice = await _db.Invoices.FindAsync(invoiceId);
        if (invoice == null)
            return ApiResponse<object>.Fail(404, "Không tìm thấy hóa đơn");

        if (request.Status == "success")
        {
            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return ApiResponse<object>.Ok(null!, "Webhook xử lý thành công");
    }

    public async Task<ApiResponse<InvoiceDto>> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        var userExists = await _db.Users.AnyAsync(u => u.UserId == request.UserId);
        if (!userExists)
        {
            var tempUser = new User
            {
                UserId = request.UserId,
                FullName = "Học viên liên kết",
                Email = $"{request.UserId}@trungtam.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = UserRole.Student,
                CreatedAt = DateTime.UtcNow
            };
            _db.Users.Add(tempUser);
            await _db.SaveChangesAsync();
        }

        var invoice = new Invoice
        {
            InvoiceId = Guid.NewGuid(),
            UserId = request.UserId,
            CourseName = request.CourseName,
            CourseId = request.CourseId,
            Amount = request.Amount,
            Status = InvoiceStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        var dbUser = await _db.Users.FindAsync(request.UserId);

        return ApiResponse<InvoiceDto>.Ok(new InvoiceDto
        {
            InvoiceId = invoice.InvoiceId,
            UserId = invoice.UserId,
            StudentName = dbUser?.FullName ?? "Học viên liên kết",
            CourseName = invoice.CourseName,
            CourseId = invoice.CourseId,
            Amount = invoice.Amount,
            Status = invoice.Status.ToString(),
            CreatedAt = invoice.CreatedAt
        }, "Tạo hóa đơn thành công");
    }
}
