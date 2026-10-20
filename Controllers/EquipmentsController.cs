using MediatR;
using Equipment.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Equipment.Api.DTOs.Errors;
using Microsoft.AspNetCore.Authorization;
using Equipment.Api.Application.Equipment.Queries;
using Equipment.Api.Application.Equipment.Commands;

namespace Equipment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentsController : ControllerBase
{
    private readonly ISender _sender;
    public EquipmentsController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] GetAllEquipmentsQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var equipment = await _sender.Send(new GetEquipmentByIdQuery(id));

        if (equipment == null) return NotFound(
            ErrorResponseFactory.Create(
                HttpContext,
                ErrorCodes.EquipmentNotFound,
                "Không tìm thấy thiết bị"
            )
        );
        return Ok(equipment);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateEquipmentRequest request)
    {
        var data = await _sender.Send(
        new CreateEquipmentCommand(
            request.Code,
            request.Name,
            request.Status,
            request.CategoryId)
        );
        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEquipmentRequest request)
    {
        var data = await _sender.Send(
            new UpdateEquipmentCommand(
                id,
                request.Code,
                request.Name,
                request.Status,
                request.CategoryId));

        if (data == null) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _sender.Send(new DeleteEquipmentCommand(id));

        if (!result) return NotFound();

        return NoContent();
    }
}
