using System.ComponentModel.DataAnnotations;

namespace Equipment.Api.DTOs;

public class CreateEquipmentRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = "";

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = "";

    [Required]
    public string Status { get; set; } = "";

    [Required]
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
