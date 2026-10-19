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

        var categoryNames = new[] { "Laptop", "Keyboard", "Mouse" };

        foreach (var name in categoryNames)
        {
            var exists = await dbContext.Categories
                .AnyAsync(c => c.Name == name);

            if (!exists)
            {
                dbContext.Categories.Add(new Category
                {
                    Name = name,
                    Description = $"Danh mục {name}"
                });
            }
        }

        await dbContext.SaveChangesAsync();

        var laptopCategory = await dbContext.Categories
            .FirstAsync(c => c.Name == "Laptop");

        var keyboardCategory = await dbContext.Categories
            .FirstAsync(c => c.Name == "Keyboard");

        var mouseCategory = await dbContext.Categories
            .FirstAsync(c => c.Name == "Mouse");

        var equipmentSamples = new[]
        {
            new { Code = "SEED-LAPTOP-01", Name = "Laptop Dell", CategoryId = laptopCategory.Id },
            new { Code = "SEED-KEYBOARD-01", Name = "Logitech Keyboard", CategoryId = keyboardCategory.Id },
            new { Code = "SEED-MOUSE-01", Name = "Logitech Mouse", CategoryId = mouseCategory.Id }
        };

        foreach (var sample in equipmentSamples)
        {
            var exists = await dbContext.Equipments
                .AnyAsync(e => e.Code == sample.Code);

            if (!exists)
            {
                dbContext.Equipments.Add(new EquipmentModel
                {
                    Code = sample.Code,
                    Name = sample.Name,
                    Status = "Available",
                    CategoryId = sample.CategoryId
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }
}