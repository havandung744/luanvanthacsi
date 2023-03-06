using luanvanthacsi.Data.Entities;
using ISession = NHibernate.ISession;
using NHibernate;
using NHibernate.Linq;
using MathNet.Numerics.Distributions;

namespace luanvanthacsi.Data.Services
{
    public class SecretaryService : ISecretaryService
    {
        public async Task<List<Secretary>> GetAllByIdAsync(string id)
        {
            try
            {
                List<Secretary> secretarys;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    secretarys = session.Query<Secretary>().Where(x => x.FacultyId == id).ToList();
                }
                return secretarys;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Secretary> GetSecretaryByIdAsync(string id)
        {
            Secretary secretary;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        secretary = await session.GetAsync<Secretary>(id);
                        return secretary;
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
