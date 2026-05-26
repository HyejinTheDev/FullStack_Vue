using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.API.DTOs.Auth;
using PaymentService.API.Services.Interfaces;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// POST /api/v1/auth/login - Đăng nhập, trả về JWT Token
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// POST /api/v1/auth/refresh-token - Cấp lại Token mới
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// POST /api/v1/auth/register - Học viên đăng ký tài khoản (Mặc định role: Student)
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// PUT /api/v1/auth/change-password - Đổi mật khẩu tài khoản hiện tại
    /// </summary>
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _authService.ChangePasswordAsync(userId, request);
        return StatusCode(result.StatusCode, result);
    }
}
