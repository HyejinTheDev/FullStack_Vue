using PaymentService.API.DTOs;
using PaymentService.API.DTOs.Users;

namespace PaymentService.API.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<UserDto>> GetMeAsync(Guid userId);
    Task<ApiResponse<List<UserDto>>> GetUsersAsync(string? role, int pageIndex, int pageSize);
    Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request);
    Task<ApiResponse<UserDto>> UpdateUserRoleAsync(Guid userId, UpdateRoleRequest request);
}
