using Microsoft.EntityFrameworkCore;
using Equipment.Api.Models;
using EquipmentModel = Equipment.Api.Models.Equipment;

namespace Equipment.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<EquipmentModel> Equipments => Set<EquipmentModel>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}
