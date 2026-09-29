using Equipment.Api.Data;
using Equipment.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using EquipmentModel = Equipment.Api.Models.Equipment;

namespace Equipment.Api.Services;

public class EquipmentService : IEquipmentService
{
    private readonly AppDbContext _dbContext;

    public EquipmentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<EquipmentModel>> GetAllAsync()
    {
        return await _dbContext.Equipments
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<EquipmentModel?> GetByIdAsync(int id)
    {
        var equipment = await _dbContext.Equipments.FirstOrDefaultAsync(eq => eq.Id == id);
        return equipment;
    }

    public async Task<EquipmentModel> CreateAsync(EquipmentModel equipment)
    {
        _dbContext.Equipments.Add(equipment);
        await _dbContext.SaveChangesAsync();
        return equipment;
    }

    public async Task<bool> UpdateAsync(int id, UpdateEquipmentRequest request)
    {
        var equipment = await _dbContext.Equipments.FirstOrDefaultAsync(eq => eq.Id == id);
        if (equipment == null)
        {
            return false;
        }
        equipment.Code = request.Code;
        equipment.Name = request.Name;
        equipment.Status = request.Status;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var equipment = await _dbContext.Equipments.FirstOrDefaultAsync(eq => eq.Id == id);

        if (equipment == null)
        {
            return false;
        }

        _dbContext.Equipments.Remove(equipment);

        await _dbContext.SaveChangesAsync();

        return true;
    }
}
