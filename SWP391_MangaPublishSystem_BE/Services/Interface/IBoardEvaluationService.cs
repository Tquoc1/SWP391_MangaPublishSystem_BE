using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Services.DTO;

namespace Services.Interface
{
    public interface IBoardEvaluationService
    {
        Task<List<BoardEvaluationDto.Response>> GetAllAsync();
        Task<BoardEvaluationDto.Response> GetByIdAsync(int id);
        Task<int> CreateAsync(BoardEvaluationDto.Create dto);
        Task<bool> UpdateAsync(int id, BoardEvaluationDto.Update dto);
        Task<bool> DeleteAsync(int id);
    }
}
