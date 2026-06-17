using DTOs;
using Entities.Models;
using Repositories.Repository;
using Services.Interface;

namespace Services.Implement
{
    public class MangakaAssistantService : IMangakaAssistantService
    {
        private readonly MangakaAssistantRepository _repository;

        public MangakaAssistantService(MangakaAssistantRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MangakaAssistantDto>> GetAllAsync(int? mangakaId, int? assistantId)
        {
            var data = await _repository.GetAllAsync();

            var query = data.Where(x => x.Isdeleted != true);

            if (mangakaId.HasValue)
            {
                query = query.Where(x => x.MangakaId == mangakaId.Value);
            }

            if (assistantId.HasValue)
            {
                query = query.Where(x => x.AssistantId == assistantId.Value);
            }

            return query.Select(MapToDto).ToList();
        }

        public async Task<MangakaAssistantDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null || entity.Isdeleted == true)
            {
                return null;
            }

            return MapToDto(entity);
        }

        public async Task<int> CreateAsync(MangakaAssistantDto.Create dto)
        {
            var existed = await _repository.ExistsActiveContractAsync(dto.MangakaId, dto.AssistantId);
            if (existed)
            {
                return 0;
            }

            var entity = new MangakaAssistant
            {
                MangakaId = dto.MangakaId,
                AssistantId = dto.AssistantId,
                SalaryAmount = dto.SalaryAmount,
                SalaryType = string.IsNullOrWhiteSpace(dto.SalaryType) ? "Monthly" : dto.SalaryType,
                ContractTerms = dto.ContractTerms,
                Status = "Pending",
                StartDate = ToDateOnly(dto.StartDate),
                EndDate = ToDateOnly(dto.EndDate),
                Createdat = DateTime.Now,
                Isdeleted = false
            };

            return await _repository.AddAsync(entity);
        }

        public async Task<int> UpdateAsync(int id, MangakaAssistantDto.Update dto)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null || entity.Isdeleted == true)
            {
                return 0;
            }

            entity.SalaryAmount = dto.SalaryAmount;
            entity.SalaryType = dto.SalaryType;
            entity.ContractTerms = dto.ContractTerms;
            entity.StartDate = ToDateOnly(dto.StartDate);
            entity.EndDate = ToDateOnly(dto.EndDate);

            return await _repository.UpdateAsync(entity);
        }

        public async Task<int> UpdateStatusAsync(int id, string status)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null || entity.Isdeleted == true)
            {
                return 0;
            }

            entity.Status = status;

            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null || entity.Isdeleted == true)
            {
                return false;
            }

            entity.Isdeleted = true;

            var result = await _repository.UpdateAsync(entity);
            return result > 0;
        }

        private MangakaAssistantDto MapToDto(MangakaAssistant entity)
        {
            return new MangakaAssistantDto
            {
                ContractId = entity.ContractId,
                MangakaId = entity.MangakaId,
                AssistantId = entity.AssistantId,
                SalaryAmount = entity.SalaryAmount,
                SalaryType = entity.SalaryType,
                ContractTerms = entity.ContractTerms,
                Status = entity.Status,
                StartDate = ToDateTime(entity.StartDate),
                EndDate = ToDateTime(entity.EndDate),
                Createdat = entity.Createdat,
                Isdeleted = entity.Isdeleted
            };
        }

        private DateOnly? ToDateOnly(DateTime? date)
        {
            return date.HasValue ? DateOnly.FromDateTime(date.Value) : null;
        }

        private DateTime? ToDateTime(DateOnly? date)
        {
            return date.HasValue ? date.Value.ToDateTime(TimeOnly.MinValue) : null;
        }
    }
}