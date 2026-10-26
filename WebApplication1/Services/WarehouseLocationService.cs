using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs.BaseDto;
using WebApplication1.DTOs.WarehouseLocation;
using WebApplication1.Infrastructure;
using WebApplication1.Infrastructure.Apis;
using System.Text.Json;

namespace WebApplication1.Services
{
    public interface IWarehouseLocationservice
    {
        Task<List<WarehouseLocationViewModel>> GetAllAsync(SavedType saved);
        Task<WarehouseLocationViewModel> GetByIdAsync(int id);
        Task<WarehouseLocationViewModel> CreateAsync(CreateWarehouseLocationDto dto);
        Task<WarehouseLocationViewModel> UpdateAsync(UpdateWarehouseLocationDto dto);
        Task<bool> DeleteAsync(string code);
    }

    public class WarehouseLocationservice : IWarehouseLocationservice
    {
        private readonly AppDbContext _context;
        private readonly ICrmApiWarehouseLocation _crmApiWarehouseLocation;


        public WarehouseLocationservice(AppDbContext context,ICrmApiWarehouseLocation CrmApiWarehouseLocation)
        {
            _context = context;
            _crmApiWarehouseLocation = CrmApiWarehouseLocation;
        }

        public async Task<List<WarehouseLocationViewModel>> GetAllAsync(SavedType saved)
        {
            if (saved == SavedType.Database)
            {
                var entities = await _context.WarehouseLocations.ToListAsync();

                return entities.Select(MapToDto).ToList();
            }

            if (saved == SavedType.CRM)
            {
                var json = await _crmApiWarehouseLocation.GetAllAsync();

                using var document = JsonDocument.Parse(json);

                var records = document.RootElement.GetProperty("value");

                var result = records.EnumerateArray()
                    .Select(record => new WarehouseLocationViewModel
                    {
                        Name = record.GetProperty("poudeh_name").GetString(),
                        Code = record.GetProperty("poudeh_code").GetString()

                    }).ToList();

                return result;
            }

            throw new ArgumentOutOfRangeException(nameof(saved));
        }

        public async Task<WarehouseLocationViewModel> GetByIdAsync(int id)
        {
            var entity = await _context.WarehouseLocations.FirstOrDefaultAsync(x => x.Id == id);
            return entity != null ? MapToDto(entity) : null;
        }

        public async Task<WarehouseLocationViewModel> CreateAsync(CreateWarehouseLocationDto dto)
        {
            var entity = new WarehouseLocation
            {
                Owner = dto.Owner,
                Name = dto.Name,
                Code = dto.Code
            };
            await _crmApiWarehouseLocation.CreateAsync(entity.Name, entity.Code);
            _context.WarehouseLocations.Add(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);

        }

        public async Task<WarehouseLocationViewModel> UpdateAsync(UpdateWarehouseLocationDto dto)
        {
            var entity = await _context.WarehouseLocations.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity != null)
            {
                entity.Owner = dto.Owner;
                entity.Name = dto.Name;
                entity.Code = dto.Code;
                _context.WarehouseLocations.Update(entity);
                await _context.SaveChangesAsync();
                return MapToDto(entity);
            }
            await _crmApiWarehouseLocation.UpdateAsync(dto);
            return new WarehouseLocationViewModel();
        }

        public async Task<bool> DeleteAsync(string code)
        {
            var entity = await _context.WarehouseLocations.FirstOrDefaultAsync(x => x.Code == code);
            if (entity == null)
                return false;
            await _crmApiWarehouseLocation.DeleteByCodeAsync(entity.Code);
            _context.WarehouseLocations.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private static WarehouseLocationViewModel MapToDto(WarehouseLocation entity)
        {
            return new WarehouseLocationViewModel
            {
                Id = entity.Id,
                Owner = entity.Owner,
                Name = entity.Name,
                Code = entity.Code
            };
        }
    }
}
