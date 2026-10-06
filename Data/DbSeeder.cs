using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Models;

namespace Equipment.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
    AppDbContext dbContext,
    PasswordHasher<User> passwordHasher,
    IConfiguration configuration)
    {
        var existingAdmin = await dbContext.Users
                            .FirstOrDefaultAsync(u => u.Username == "admin");

        if (existingAdmin != null)
        {
            return;
        }

        var adminPassword = configuration["SeedAdmin:Password"];

        var admin = new User
        {
            Username = "admin",
            Role = "Admin"
        };

        admin.PasswordHash = passwordHasher.HashPassword(admin, adminPassword!);

        dbContext.Users.Add(admin);

        await dbContext.SaveChangesAsync();
    }
}