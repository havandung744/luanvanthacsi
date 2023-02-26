using luanvanthacsi.Data.Entities;
using NHibernate;

namespace luanvanthacsi.Data.Services
{
    public class EvaluationBoardService : IEvaluationBoardService
    {
        public async Task<bool> AddOrUpdateEvaluationBoard(EvaluationBoard evaluationBoard)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(evaluationBoard.Id))
                        {
                            evaluationBoard.Id = Guid.NewGuid().ToString();
                            evaluationBoard.CreateDate = DateTime.Now;
                            evaluationBoard.UpdateDate = DateTime.Now;
                            await session.SaveAsync(evaluationBoard);
                        }
                        else
                        {
                            evaluationBoard.UpdateDate = DateTime.Now;
                            await session.UpdateAsync(evaluationBoard);
                        }
                        await transaction.CommitAsync();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
                return result;
            }
        }

        public async Task<bool> DeleteEvaluationBoardAsync(EvaluationBoard evaluationBoard)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {

                        await session.DeleteAsync(evaluationBoard);
                        await transaction.CommitAsync();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }
            return result;
        }

        public async Task<bool> DeleteEvaluationBoardAsync(List<EvaluationBoard> evaluationBoard)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in evaluationBoard)
                        {
                            await session.DeleteAsync(item);
                        }
                        await transaction.CommitAsync();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }
            return result;
        }

        public async Task<List<EvaluationBoard>> GetAllByIdAsync(string id)
        {
            try
            {
                List<EvaluationBoard> evaluationBoard;
                using (NHibernate.ISession session = FluentNHibernateHelper.OpenSession())
                {
                    evaluationBoard = session.Query<EvaluationBoard>().Where(x => x.FacultyId == id).ToList();
                }
                return evaluationBoard;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<EvaluationBoard> GetEvaluationBoardByIdAsync(string id)
        {
            EvaluationBoard evaluationBoard;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        evaluationBoard = await session.GetAsync<EvaluationBoard>(id);
                        return evaluationBoard;
                    }
                    catch (Exception ex)
                    {
                        //await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }
        }
    }
}
