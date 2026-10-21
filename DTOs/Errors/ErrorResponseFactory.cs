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


    export async function getApiError(
        res: Response
    ): Promise<ApiErrorResponse> {
    const fallback: ApiErrorResponse = {
        code: "UNEXPECTED_ERROR",
        message: "Đã xảy ra lỗi. Vui lòng thử lại.",
        traceId: "",
        timestamp: new Date().toISOString()
    };

    try {
        const body = await res.text();

        if (!body) {
            return fallback;
        }

        const contentType = res.headers.get("content-type") ?? "";

if (contentType.includes("application/json"))
{
    const data: unknown = JSON.parse(body);

    if (
        typeof data !== "object" ||
        data === null ||
        !("code" in data) ||
        typeof data.code !== "string" ||
        !data.code.trim() ||
        !("message" in data) ||
        typeof data.message !== "string" ||
        !data.message.trim()
            ) {
        return fallback;
    }

    return {
    code: data.code,
                message: data.message,
                traceId:
        "traceId" in data && typeof data.traceId === "string"
            ? data.traceId
            : "",
                timestamp:
        "timestamp" in data &&
        typeof data.timestamp === "string"
            ? data.timestamp
            : new Date().toISOString()
            }
    ;
}

return {
    ...fallback,
            message: body
        }
;
    } catch {
    return fallback;
}
}


    private static string GetTraceId(HttpContext httpContext)
{
    return Activity.Current?.Id ?? httpContext.TraceIdentifier;
}
}
