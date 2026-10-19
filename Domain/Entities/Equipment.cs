namespace Equipment.Api.Domain.Entities;

public class Equipment
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}