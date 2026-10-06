namespace Equipment.Api.DTOs.Errors;

/// <summary>
/// Danh sách mã lỗi dùng chung giữa backend và frontend.
/// Bổ sung mã mới theo nghiệp vụ thay vì hard-code chuỗi trong controller.
/// </summary>
public static class ErrorCodes
{
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string ResourceNotFound = "RESOURCE_NOT_FOUND";
    public const string UsernameAlreadyExists = "USERNAME_ALREADY_EXISTS";
    public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
    public const string UnexpectedError = "UNEXPECTED_ERROR";
}
