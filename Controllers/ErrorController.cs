using Microsoft.AspNetCore.Mvc;

namespace Equipment.Api.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    [HttpGet]
    [Route("/error")]
    public IActionResult Error()
    {
        return Problem();
    }
}
