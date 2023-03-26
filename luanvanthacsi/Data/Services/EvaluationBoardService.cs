using luanvanthacsi.Data.Entities;
using NHibernate;
using NHibernate.Linq;
using static NHibernate.Engine.Query.CallableParser;

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
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
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
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
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
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                    }
                }
            }
            return result;
        }

        public async Task<List<EvaluationBoard>> GetAllAsync()
        {
            try
            {
                List<EvaluationBoard> evaluationBoard;
                using (NHibernate.ISession session = FluentNHibernateHelper.OpenSession())
                {
                    evaluationBoard = session.Query<EvaluationBoard>().ToList();
                }
                return evaluationBoard;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EvaluationBoard>> GetAllByIdAsync(string id)
        {
            try
            {
                List<EvaluationBoard> evaluationBoard;
                using (NHibernate.ISession session = FluentNHibernateHelper.OpenSession())
                {
                    evaluationBoard = await session.Query<EvaluationBoard>().Where(x => x.FacultyId == id).ToListAsync();
                    if (evaluationBoard.Any())
                    {
                        var listFacltyIds = evaluationBoard.Select(c => c.FacultyId);
                        var faculty = await session.Query<Faculty>().Where(c => listFacltyIds.Contains(c.Id)).ToListAsync();
                        evaluationBoard = evaluationBoard.GroupJoin(faculty, ev => ev.FacultyId, fa => fa.Id, (ev, fa) => (ev, fa))
                        .Select(x =>
                        {
                            x.ev.Faculty = x.fa.FirstOrDefault();
                            return x.ev;
                        }).ToList();

                    }
                }
                return evaluationBoard;
            }
            catch (Exception)
            {
                throw;
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
                    catch (Exception)
                    {
                        //await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
    }
}
