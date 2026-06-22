using DTOs;
using Entities.Models;
using Repositories.Repository;
using Services.Interface;

namespace Services.Implement
{
    public class MangakaAssistantService : IMangakaAssistantService
    {
        private static readonly Dictionary<string, List<string>> _validTransitions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Pending", new List<string> { "Active", "Terminated" } },
            { "Active", new List<string> { "Suspended", "Terminated", "Expired", "Completed" } },
            { "Suspended", new List<string> { "Active", "Terminated" } }
        };

        private readonly MangakaAssistantRepository _repository;
        private readonly INotificationService _notificationService;

        public MangakaAssistantService(MangakaAssistantRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
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
                throw new InvalidOperationException("This assistant is already assigned to the mangaka!!!");
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

            var result = await _repository.AddAsync(entity);
            if (result > 0)
            {
                await _notificationService.CreateNotificationAsync(
                    dto.AssistantId,
                    "Lời mời hợp tác mới",
                    $"Một Mangaka vừa gửi cho bạn một lời mời hợp tác."
                );
            }
            return result;
        }

        public async Task UpdateAsync(int id, MangakaAssistantDto.Update dto)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null || entity.Isdeleted == true)
            {
                throw new KeyNotFoundException("Contract not found !!!");
            }

            entity.SalaryAmount = dto.SalaryAmount;
            entity.SalaryType = dto.SalaryType;
            entity.ContractTerms = dto.ContractTerms;
            entity.StartDate = ToDateOnly(dto.StartDate);
            entity.EndDate = ToDateOnly(dto.EndDate);

            await _repository.UpdateAsync(entity);
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null || entity.Isdeleted == true)
            {
                throw new KeyNotFoundException("Contract not found!!!");
            }

            if (string.Equals(entity.Status, status, StringComparison.OrdinalIgnoreCase))
                return;

            if (!_validTransitions.ContainsKey(entity.Status) || 
                !_validTransitions[entity.Status].Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ '{entity.Status}' sang '{status}'. Luồng không hợp lệ!");
            }

            entity.Status = status;

            await _repository.UpdateAsync(entity);
            
            string action = status == "Accepted" ? "chấp nhận" : (status == "Rejected" ? "từ chối" : "cập nhật");
            await _notificationService.CreateNotificationAsync(
                entity.MangakaId,
                "Phản hồi lời mời hợp tác",
                $"Một Assistant đã {action} lời mời của bạn."
            );
        }

        public async Task RemoveAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null || entity.Isdeleted == true)
            {
                throw new KeyNotFoundException("Contract not found!!!");
            }

            entity.Isdeleted = true;

            await _repository.UpdateAsync(entity);
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