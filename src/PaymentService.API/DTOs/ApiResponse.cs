namespace PaymentService.API.DTOs;

/// <summary>
/// Wrapper chuẩn cho tất cả API responses
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<FieldError>? Errors { get; set; }
    public PaginationMeta? Pagination { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string message = "Thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = 200,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Created(T data, string message = "Tạo thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = 201,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(int statusCode, string message, List<FieldError>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}

public class FieldError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class PaginationMeta
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
