using Equipment.Api.DTOs;
using Equipment.Api.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EquipmentModel = Equipment.Api.Models.Equipment;

namespace Equipment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentsController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;
    public EquipmentsController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _equipmentService.GetAllAsync();
        return Ok(data);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var equipment = await _equipmentService.GetByIdAsync(id);
        if (equipment == null) return NotFound();
        return Ok(equipment);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateEquipmentRequest request)
    {
        var equipment = new EquipmentModel
        {
            Code = request.Code,
            Name = request.Name,
            Status = request.Status
        };
        var data = await _equipmentService.CreateAsync(equipment);
        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEquipmentRequest request)
    {
        var result = await _equipmentService.UpdateAsync(id, request);
        if (!result) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _equipmentService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
