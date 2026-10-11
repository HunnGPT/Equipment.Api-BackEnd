using Microsoft.AspNetCore.Mvc;
using Equipment.Api.DTOs.Errors;
using Microsoft.AspNetCore.Diagnostics;

namespace Equipment.Api.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("/error")]
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        _logger.LogError(
            exception,
            "Unhandled exception. TraceId: {TraceId}",
            HttpContext.TraceIdentifier
        );

        var response = ErrorResponseFactory.Create(
            HttpContext,
            ErrorCodes.UnexpectedError,
            "Đã xảy ra lỗi"
        );

        return StatusCode(
            StatusCodes.Status500InternalServerError,
            response
        );
    }
}
