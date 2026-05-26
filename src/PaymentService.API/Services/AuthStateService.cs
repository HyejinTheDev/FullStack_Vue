using PaymentService.API.DTOs.Users;
using PaymentService.API.Enums;

namespace PaymentService.API.Services;

/// <summary>
/// Service quản lý trạng thái đăng nhập cho Blazor Server (scoped per circuit).
/// Trong Blazor Server, mỗi user có 1 circuit riêng nên scoped service hoạt động như session.
/// </summary>
public class AuthStateService
{
    public bool IsLoggedIn { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = "";
    public string UserEmail { get; private set; } = "";
    public string UserRole { get; private set; } = "";
    public bool IsAdmin => UserRole == "Admin";

    public event Action? OnChange;

    public void Login(Guid userId, string fullName, string email, string role)
    {
        UserId = userId;
        UserName = fullName;
        UserEmail = email;
        UserRole = role;
        IsLoggedIn = true;
        OnChange?.Invoke();
    }

    public void Logout()
    {
        UserId = Guid.Empty;
        UserName = "";
        UserEmail = "";
        UserRole = "";
        IsLoggedIn = false;
        OnChange?.Invoke();
    }
}
