# Hướng dẫn sử dụng Error DTO

## 1. Mục tiêu

Bộ Error DTO giúp tất cả endpoint trả lỗi theo một cấu trúc thống nhất. Frontend không cần đoán response là chuỗi, JSON hay `ProblemDetails`, đồng thời có thể dùng `code` để xử lý logic và `traceId` để tra cứu log backend.

Các tệp liên quan:

- `DTOs/Errors/ApiErrorResponse.cs`: lỗi HTTP hoặc lỗi nghiệp vụ thông thường.
- `DTOs/Errors/ApiValidationErrorResponse.cs`: lỗi validation theo từng field.
- `DTOs/Errors/ErrorCodes.cs`: danh sách mã lỗi dùng chung.
- `DTOs/Errors/ErrorResponseFactory.cs`: factory tạo response và tự lấy `traceId`.

## 2. Cấu trúc response

### Lỗi thông thường

```json
{
  "code": "INVALID_CREDENTIALS",
  "message": "Tên đăng nhập hoặc mật khẩu không đúng",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestamp": "2026-10-06T03:00:00+00:00"
}
```

Ý nghĩa các field:

| Field | Ý nghĩa |
|---|---|
| `code` | Mã lỗi ổn định để frontend xử lý bằng code. |
| `message` | Thông báo có thể hiển thị cho người dùng. |
| `traceId` | Mã dùng để tìm request tương ứng trong log backend. |
| `timestamp` | Thời điểm server tạo response, theo UTC. |

### Lỗi validation

```json
{
  "code": "VALIDATION_FAILED",
  "message": "Dữ liệu đầu vào không hợp lệ",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestamp": "2026-10-06T03:00:00+00:00",
  "errors": {
    "username": [
      "Username không được để trống"
    ],
    "password": [
      "Password phải có ít nhất 8 ký tự"
    ]
  }
}
```

`errors` là một dictionary:

- Key: tên field bị lỗi.
- Value: danh sách thông báo lỗi của field đó.

## 3. Nguyên tắc sử dụng

### Frontend xử lý bằng `code`, không xử lý bằng `message`

Đúng:

```ts
if (error.code === "INVALID_CREDENTIALS") {
    // Hiển thị lỗi đăng nhập.
}
```

Không nên:

```ts
if (error.message === "Sai tên tài khoản hoặc mật khẩu") {
    // Không ổn định vì message có thể được sửa hoặc dịch.
}
```

### Không đưa thông tin nội bộ vào `message`

Không trả cho client:

- Stack trace.
- Connection string.
- SQL statement.
- Tên bảng hoặc cấu trúc database không cần thiết.
- Nội dung exception kỹ thuật.

Thông tin kỹ thuật cần được ghi vào log backend cùng `traceId`.

### Mỗi loại lỗi có một mã ổn định

Mã lỗi dùng dạng chữ in hoa và dấu gạch dưới:

```text
INVALID_CREDENTIALS
EQUIPMENT_NOT_FOUND
EQUIPMENT_CODE_ALREADY_EXISTS
INVALID_REFRESH_TOKEN
```

Không dùng status code làm mã nghiệp vụ. Nhiều lỗi khác nhau có thể cùng trả HTTP `409`, nhưng cần các `code` riêng để frontend phân biệt.

## 4. Sử dụng trong Controller

Thêm namespace:

```csharp
using Equipment.Api.DTOs.Errors;
```

### Trả lỗi đăng nhập

Thay vì:

```csharp
return Unauthorized("Sai tên tài khoản hoặc mật khẩu");
```

Sử dụng:

```csharp
return Unauthorized(ErrorResponseFactory.Create(
    HttpContext,
    ErrorCodes.InvalidCredentials,
    "Tên đăng nhập hoặc mật khẩu không đúng"
));
```

Kết quả trả về có HTTP status `401` và body theo `ApiErrorResponse`.

### Trả lỗi không tìm thấy dữ liệu

```csharp
if (equipment == null)
{
    return NotFound(ErrorResponseFactory.Create(
        HttpContext,
        ErrorCodes.ResourceNotFound,
        "Không tìm thấy thiết bị"
    ));
}
```

Khi dự án có nhiều resource, nên bổ sung mã cụ thể vào `ErrorCodes`:

```csharp
public const string EquipmentNotFound = "EQUIPMENT_NOT_FOUND";
```

Sau đó sử dụng:

```csharp
return NotFound(ErrorResponseFactory.Create(
    HttpContext,
    ErrorCodes.EquipmentNotFound,
    "Không tìm thấy thiết bị"
));
```

### Trả lỗi trùng username

```csharp
return Conflict(ErrorResponseFactory.Create(
    HttpContext,
    ErrorCodes.UsernameAlreadyExists,
    "Username đã tồn tại"
));
```

### Trả lỗi refresh token

```csharp
return Unauthorized(ErrorResponseFactory.Create(
    HttpContext,
    ErrorCodes.InvalidRefreshToken,
    "Refresh token không hợp lệ hoặc đã hết hạn"
));
```

### Trả lỗi validation thủ công

```csharp
var errors = new Dictionary<string, string[]>
{
    ["status"] = ["Trạng thái thiết bị không hợp lệ"],
    ["code"] = ["Mã thiết bị đã tồn tại"]
};

return BadRequest(ErrorResponseFactory.CreateValidation(
    HttpContext,
    errors
));
```

Chỉ validation thủ công khi rule không thể hoặc không nên biểu diễn bằng data annotation. Các validation đơn giản như bắt buộc và độ dài vẫn nên đặt trên request DTO.

## 5. Chuẩn hóa validation tự động của ASP.NET Core

Với `[ApiController]`, ASP.NET Core tự trả `400` khi request DTO không hợp lệ. Mặc định response này chưa sử dụng `ApiValidationErrorResponse`.

Để chuẩn hóa, thêm namespace vào `Program.cs`:

```csharp
using Equipment.Api.DTOs.Errors;
using Microsoft.AspNetCore.Mvc;
```

Sau dòng `builder.Services.AddControllers();`, thêm:

```csharp
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .ToDictionary(
                item => ToCamelCase(item.Key),
                item => item.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Giá trị không hợp lệ"
                        : error.ErrorMessage)
                    .ToArray()
            );

        var response = ErrorResponseFactory.CreateValidation(
            context.HttpContext,
            errors
        );

        return new BadRequestObjectResult(response);
    };
});
```

Ví dụ helper đổi tên field sang camelCase:

```csharp
static string ToCamelCase(string value)
{
    if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
    {
        return value;
    }

    return char.ToLowerInvariant(value[0]) + value[1..];
}
```

Helper có thể được chuyển sang class riêng nếu được sử dụng ở nhiều nơi.

## 6. Sử dụng với global exception handler

`ApiErrorResponse` nên được dùng cho lỗi không được controller xử lý. Exception handler phải ghi log chi tiết nhưng chỉ trả thông báo an toàn cho client.

Ví dụ cập nhật `ErrorController`:

```csharp
using Equipment.Api.DTOs.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Equipment.Api.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error()
    {
        var exception = HttpContext.Features
            .Get<IExceptionHandlerFeature>()?
            .Error;

        _logger.LogError(
            exception,
            "Unhandled exception. TraceId: {TraceId}",
            HttpContext.TraceIdentifier
        );

        var response = ErrorResponseFactory.Create(
            HttpContext,
            ErrorCodes.UnexpectedError,
            "Hệ thống đang gặp sự cố. Vui lòng thử lại sau"
        );

        return StatusCode(StatusCodes.Status500InternalServerError, response);
    }
}
```

Không dùng nội dung `exception.Message` làm `message` trả về client trong môi trường production.

## 7. Mapping HTTP status và error code

| HTTP status | Trường hợp | Error code ví dụ |
|---:|---|---|
| `400` | Request DTO không hợp lệ | `VALIDATION_FAILED` |
| `401` | Sai thông tin đăng nhập | `INVALID_CREDENTIALS` |
| `401` | Refresh token không hợp lệ | `INVALID_REFRESH_TOKEN` |
| `401` | Chưa xác thực | `UNAUTHORIZED` |
| `403` | Đã đăng nhập nhưng không đủ quyền | `FORBIDDEN` |
| `404` | Không tìm thấy thiết bị | `EQUIPMENT_NOT_FOUND` |
| `409` | Username đã tồn tại | `USERNAME_ALREADY_EXISTS` |
| `409` | Mã thiết bị đã tồn tại | `EQUIPMENT_CODE_ALREADY_EXISTS` |
| `500` | Lỗi ngoài dự kiến | `UNEXPECTED_ERROR` |

Phân biệt:

- `401`: người dùng chưa được xác thực hoặc thông tin xác thực không còn hợp lệ.
- `403`: người dùng đã được xác thực nhưng không có quyền thực hiện hành động.

## 8. Đọc error response ở frontend

Khai báo type tương ứng:

```ts
export type ApiErrorResponse = {
    code: string;
    message: string;
    traceId: string;
    timestamp: string;
    errors?: Record<string, string[]>;
};
```

Hàm đọc lỗi an toàn:

```ts
export async function getApiError(res: Response): Promise<ApiErrorResponse> {
    const contentType = res.headers.get("content-type") ?? "";

    if (contentType.includes("application/json")) {
        return await res.json() as ApiErrorResponse;
    }

    return {
        code: "UNEXPECTED_ERROR",
        message: (await res.text()) || "Đã xảy ra lỗi",
        traceId: "",
        timestamp: new Date().toISOString()
    };
}
```

Sử dụng:

```ts
const res = await apiFetch("/api/equipments", options);

if (!res.ok) {
    const error = await getApiError(res);

    console.error("API error", {
        code: error.code,
        traceId: error.traceId
    });

    setMessage(error.message);
    setFieldErrors(error.errors ?? {});
    return;
}
```

Frontend không nên hiển thị `traceId` thay cho thông báo lỗi, nhưng có thể hiển thị dưới dạng mã hỗ trợ khi gặp lỗi `500`.

## 9. Quy trình thêm một loại lỗi mới

Ví dụ cần thêm lỗi mã thiết bị bị trùng:

1. Thêm constant vào `ErrorCodes.cs`:

   ```csharp
   public const string EquipmentCodeAlreadyExists
       = "EQUIPMENT_CODE_ALREADY_EXISTS";
   ```

2. Chọn HTTP status phù hợp, trong trường hợp này là `409 Conflict`.

3. Trả response bằng factory:

   ```csharp
   return Conflict(ErrorResponseFactory.Create(
       HttpContext,
       ErrorCodes.EquipmentCodeAlreadyExists,
       "Mã thiết bị đã tồn tại"
   ));
   ```

4. Bổ sung integration test kiểm tra status và body.

5. Frontend chỉ thêm xử lý riêng nếu hành vi của lỗi này khác lỗi thông thường.

## 10. Checklist review

Trước khi hoàn thành một endpoint, kiểm tra:

- [ ] Không trả lỗi dưới dạng plain text.
- [ ] HTTP status phù hợp với loại lỗi.
- [ ] `code` đã được khai báo trong `ErrorCodes`.
- [ ] `message` an toàn để hiển thị cho người dùng.
- [ ] Response có `traceId`.
- [ ] Không trả stack trace hoặc thông tin nhạy cảm.
- [ ] Validation theo field sử dụng `ApiValidationErrorResponse`.
- [ ] Có test kiểm tra cả HTTP status và error code.
- [ ] Frontend không so sánh logic bằng nội dung `message`.

## 11. Phạm vi hiện tại

Bộ DTO và factory đã tồn tại trong project, nhưng các controller và validation pipeline hiện tại chưa được chuyển đổi toàn bộ sang định dạng mới. Nên triển khai theo thứ tự:

1. Chuẩn hóa lỗi login, refresh và logout.
2. Chuẩn hóa lỗi not found của Equipment.
3. Cấu hình validation tự động.
4. Cập nhật global exception handler.
5. Cập nhật `getApiError` ở frontend.
6. Bổ sung integration test.
