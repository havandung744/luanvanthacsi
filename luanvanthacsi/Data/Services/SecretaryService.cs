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
    }
}
