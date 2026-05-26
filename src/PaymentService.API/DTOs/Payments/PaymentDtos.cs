namespace PaymentService.API.DTOs.Payments;

/// <summary>
/// Webhook IPN callback từ VNPay/Momo (mô phỏng)
/// </summary>
public class PaymentWebhookRequest
{
    public string InvoiceId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty; // "success" hoặc "failed"
    public string Signature { get; set; } = string.Empty; // Chữ ký xác thực (mô phỏng)
}
