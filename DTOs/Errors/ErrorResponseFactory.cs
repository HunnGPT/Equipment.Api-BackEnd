using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Equipment.Api.DTOs.Errors;

/// <summary>
/// Factory mẫu giúp controller và exception handler tạo error response nhất quán.
/// </summary>
public static class ErrorResponseFactory
{
    public static ApiErrorResponse Create(
        HttpContext httpContext,
        string code,
        string message)
    {
        return new ApiErrorResponse
        {
            Code = code,
            Message = message,
            TraceId = GetTraceId(httpContext)
        };
    }

    public static ApiValidationErrorResponse CreateValidation(
        HttpContext httpContext,
        IReadOnlyDictionary<string, string[]> errors)
    {
        return new ApiValidationErrorResponse
        {
            Code = ErrorCodes.ValidationFailed,
            Message = "Dữ liệu đầu vào không hợp lệ",
            TraceId = GetTraceId(httpContext),
            Errors = errors
        };
    }

    private static string GetTraceId(HttpContext httpContext)
    {
        return Activity.Current?.Id ?? httpContext.TraceIdentifier;
    }
}
