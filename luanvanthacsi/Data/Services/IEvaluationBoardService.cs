using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface IEvaluationBoardService
    {
        Task<List<EvaluationBoard>> GetAllAsync();
        Task<List<EvaluationBoard>> GetAllByIdAsync(string id);
        Task<bool> AddOrUpdateEvaluationBoard(EvaluationBoard evaluationBoard);
        Task<EvaluationBoard> GetEvaluationBoardByIdAsync(string id);
        Task<bool> DeleteEvaluationBoardAsync(EvaluationBoard evaluationBoard);
        Task<bool> DeleteEvaluationBoardAsync(List<EvaluationBoard> evaluationBoard);
    }
}
