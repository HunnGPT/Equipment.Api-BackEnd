using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Domain.Entities;

using EquipmentModel = Equipment.Api.Domain.Entities.Equipment;

namespace Equipment.Api.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext dbContext,
        PasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        var existingAdmin = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == "admin");

        if (existingAdmin == null)
        {
            var adminPassword = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "SeedAdmin:Password chưa được cấu hình.");
            }

            var admin = new User
            {
                Username = "admin",
                Role = "Admin"
            };

            admin.PasswordHash = passwordHasher.HashPassword(
                admin,
                adminPassword);

            dbContext.Users.Add(admin);
        }
    }
}