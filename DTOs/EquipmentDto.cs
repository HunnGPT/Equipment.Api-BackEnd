namespace Equipment.Api.DTOs;

public class EquipmentDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
}