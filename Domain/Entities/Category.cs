namespace Equipment.Api.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}