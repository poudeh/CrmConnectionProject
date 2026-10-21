using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain;
using WebApplication1.DTOs.DefaultDescription;
using WebApplication1.Infrastructure;

namespace WebApplication1.Services
{
    public interface IDefaultDescriptionService
    {
        Task<List<DefaultDescriptionViewModel>> GetAllAsync();
        Task<DefaultDescriptionViewModel> GetByIdAsync(int id);
        Task<DefaultDescriptionViewModel> CreateAsync(CreateDefaultDescriptionDto dto);
        Task<DefaultDescriptionViewModel> UpdateAsync(UpdateDefaultDescriptionDto dto);
        Task<bool> DeleteAsync(string code);
    }

    public class DefaultDescriptionService : IDefaultDescriptionService
    {
        private readonly AppDbContext _context;
        private readonly ICrmApiService _crmApiService;


        public DefaultDescriptionService(AppDbContext context,ICrmApiService crmApiService)
        {
            _context = context;
            _crmApiService = crmApiService;
        }

        public async Task<List<DefaultDescriptionViewModel>> GetAllAsync()
        {
            var entities = await _context.DefaultDescriptions.ToListAsync();
            return entities.Select(MapToDto).ToList();
        }

        public async Task<DefaultDescriptionViewModel> GetByIdAsync(int id)
        {
            var entity = await _context.DefaultDescriptions.FirstOrDefaultAsync(x => x.Id == id);
            return entity != null ? MapToDto(entity) : null;
        }

        public async Task<DefaultDescriptionViewModel> CreateAsync(CreateDefaultDescriptionDto dto)
        {
            var entity = new DefaultDescription
            {
                Owner = dto.Owner,
                Name = dto.Name,
                Code = dto.Code
            };
            await _crmApiService.CreateAsync(entity.Name, entity.Code);
            _context.DefaultDescriptions.Add(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);

        }

        public async Task<DefaultDescriptionViewModel> UpdateAsync(UpdateDefaultDescriptionDto dto)
        {
            var entity = await _context.DefaultDescriptions.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity == null)
                return null;

            entity.Owner = dto.Owner;
            entity.Name = dto.Name;
            entity.Code = dto.Code;

            _context.DefaultDescriptions.Update(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(string code)
        {
            var entity = await _context.DefaultDescriptions.FirstOrDefaultAsync(x => x.Code == code);
            if (entity == null)
                return false;
            await _crmApiService.DeleteByCodeAsync(entity.Code);
            _context.DefaultDescriptions.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private static DefaultDescriptionViewModel MapToDto(DefaultDescription entity)
        {
            return new DefaultDescriptionViewModel
            {
                Id = entity.Id,
                Owner = entity.Owner,
                Name = entity.Name,
                Code = entity.Code
            };
        }
    }
}
