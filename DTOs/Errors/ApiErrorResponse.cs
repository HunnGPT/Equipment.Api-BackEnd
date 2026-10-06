namespace Equipment.Api.DTOs.Errors;

/// <summary>
/// Response lỗi chuẩn dùng cho các lỗi nghiệp vụ và lỗi HTTP thông thường.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// Mã lỗi ổn định để client xử lý bằng code, không phụ thuộc nội dung Message.
    /// </summary>
    public string Code { get; init; } = ErrorCodes.UnexpectedError;

    /// <summary>
    /// Thông báo có thể hiển thị cho người dùng.
    /// </summary>
    public string Message { get; init; } = "Đã xảy ra lỗi";

    /// <summary>
    /// Mã truy vết dùng để đối chiếu với log phía server.
    /// </summary>
    public string TraceId { get; init; } = "";

    /// <summary>
    /// Thời điểm server tạo response lỗi.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
