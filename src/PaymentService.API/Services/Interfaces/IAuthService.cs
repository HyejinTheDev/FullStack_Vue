using PaymentService.API.DTOs;
using PaymentService.API.DTOs.Auth;

namespace PaymentService.API.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse<object>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<object>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
}
