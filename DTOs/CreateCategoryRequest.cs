using System.ComponentModel.DataAnnotations;

namespace Equipment.Api.DTOs;

public class CreateCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(500)]
    public string? Description { get; set; }
}