using Microsoft.AspNetCore.Mvc;
using Equipment.Api.DTOs.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Equipment.Api.Application.Category.Exceptions;

namespace Equipment.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [Route("/error")]
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        if (exception is CategoryAlreadyExistsException)
        {
            var response = ErrorResponseFactory.Create(
                HttpContext,
                ErrorCodes.CategoryAlreadyExists,
                "Tên danh mục đã tồn tại"
            );

            return Conflict(response);
        }

        if (exception is CategoryInUseException)
        {
            var response = ErrorResponseFactory.Create(
                HttpContext,
                ErrorCodes.CategoryInUse,
                "Không thể xóa danh mục vì vẫn còn thiết bị đang sử dụng"
            );

            return Conflict(response);
        }

        if (exception is CategoryNotFoundException)
        {
            var response = ErrorResponseFactory.Create(
                HttpContext,
                ErrorCodes.CategoryNotFound,
                "Không tìm thấy danh mục"
            );

            return NotFound(response);
        }

        _logger.LogError(
            exception,
            "Unhandled exception. TraceId: {TraceId}",
            HttpContext.TraceIdentifier
        );

        var errorResponse = ErrorResponseFactory.Create(
            HttpContext,
            ErrorCodes.UnexpectedError,
            "Đã xảy ra lỗi"
        );

        return StatusCode(
            StatusCodes.Status500InternalServerError,
            errorResponse
        );
    }
}
