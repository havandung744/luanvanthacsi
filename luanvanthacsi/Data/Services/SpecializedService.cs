using luanvanthacsi.Data.Entities;
using ISession = NHibernate.ISession;
using NHibernate;
using NHibernate.Linq;
using MathNet.Numerics.Distributions;
using NHibernate.Mapping;
using LightInject;
using AutoMapper;
using MathNet.Numerics.Optimization;

namespace luanvanthacsi.Data.Services
{
    public class SpecializedService : ISpecializedService
    {
        public async Task<List<Specialized>> GetAllAsync()
        {
            try
            {
                List<Specialized> specializeds;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    specializeds = session.Query<Specialized>().ToList();
                }
                return specializeds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Specialized> GetByIdAsync(string id)
        {
            Specialized specialized;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        specialized = await session.GetAsync<Specialized>(id);
                        return specialized;
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
