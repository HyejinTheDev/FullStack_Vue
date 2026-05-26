using Microsoft.EntityFrameworkCore;
using PaymentService.API.Data;
using PaymentService.API.DTOs;
using PaymentService.API.DTOs.Users;
using PaymentService.API.Entities;
using PaymentService.API.Enums;
using PaymentService.API.Services.Interfaces;

namespace PaymentService.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<UserDto>> GetMeAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return ApiResponse<UserDto>.Fail(404, "Không tìm thấy user");

        return ApiResponse<UserDto>.Ok(MapToDto(user), "Lấy thông tin thành công");
    }

    public async Task<ApiResponse<List<UserDto>>> GetUsersAsync(string? role, int pageIndex, int pageSize)
    {
        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, true, out var parsedRole))
        {
            query = query.Where(u => u.Role == parsedRole);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = ApiResponse<List<UserDto>>.Ok(
            users.Select(MapToDto).ToList(),
            "Lấy danh sách user thành công"
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

    public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
        {
            return ApiResponse<UserDto>.Fail(400, "Email đã tồn tại", new List<FieldError>
            {
                new() { Field = "Email", Message = "Email này đã được sử dụng" }
            });
        }

        if (!Enum.TryParse<UserRole>(request.Role, true, out var parsedRole))
        {
            return ApiResponse<UserDto>.Fail(400, "Role không hợp lệ", new List<FieldError>
            {
                new() { Field = "Role", Message = "Role phải là Teacher hoặc Staff" }
            });
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = parsedRole
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return ApiResponse<UserDto>.Created(MapToDto(user), "Tạo tài khoản thành công");
    }

    public async Task<ApiResponse<UserDto>> UpdateUserRoleAsync(Guid userId, UpdateRoleRequest request)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return ApiResponse<UserDto>.Fail(404, "Không tìm thấy user");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var parsedRole))
        {
            return ApiResponse<UserDto>.Fail(400, "Role không hợp lệ");
        }

        user.Role = parsedRole;
        await _db.SaveChangesAsync();

        return ApiResponse<UserDto>.Ok(MapToDto(user), "Cập nhật role thành công");
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
