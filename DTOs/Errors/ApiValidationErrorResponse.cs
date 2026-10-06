namespace Equipment.Api.DTOs.Errors;

/// <summary>
/// Response dành cho lỗi validation theo từng field.
/// </summary>
public sealed class ApiValidationErrorResponse : ApiErrorResponse
{
    /// <summary>
    /// Key là tên field; value là danh sách thông báo lỗi của field đó.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; init; }
        = new Dictionary<string, string[]>();
}
