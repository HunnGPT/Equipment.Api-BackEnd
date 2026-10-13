using Equipment.Api.DTOs;
using EquipmentModel = Equipment.Api.Domain.Entities.Equipment;

namespace Equipment.Api.Domain.Interfaces;

public interface IEquipmentService
{
    Task<List<EquipmentModel>> GetAllAsync();
    Task<EquipmentModel?> GetByIdAsync(int id);
    Task<EquipmentModel> CreateAsync(EquipmentModel equipment);
    Task<bool> UpdateAsync(int id, UpdateEquipmentRequest request);
    Task<bool> DeleteAsync(int id);
}
