using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.API.DTOs.Users;
using PaymentService.API.Services.Interfaces;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// GET /api/v1/users/me - Lấy profile user đang đăng nhập
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _userService.GetMeAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// GET /api/v1/users?role=Teacher - (Admin) Lấy danh sách user
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers([FromQuery] string? role, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _userService.GetUsersAsync(role, pageIndex, pageSize);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// POST /api/v1/users - (Admin) Tạo tài khoản hệ thống (Giáo viên, Staff)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// PUT /api/v1/users/{userId}/roles - (Admin) Cập nhật Role cho một user
    /// </summary>
    [HttpPut("{userId}/roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(Guid userId, [FromBody] UpdateRoleRequest request)
    {
        var result = await _userService.UpdateUserRoleAsync(userId, request);
        return StatusCode(result.StatusCode, result);
    }
}
