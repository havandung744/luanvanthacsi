using luanvanthacsi.Data.Entities;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace luanvanthacsi.Data.Services
{
    public class ScientistService : IScientistService
    {
        public async Task<List<Scientist>> GetAll()
        {
            try
            {
                List<Scientist> scientists;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    scientists = session.Query<Scientist>().ToList();
                }
                return scientists;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> AddOrUpdateScientist(Scientist scientist)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(scientist.Id))
                        {
                            scientist.Id = Guid.NewGuid().ToString();
                            await session.SaveAsync(scientist);
                        }
                        else
                        {
                            await session.UpdateAsync(scientist);
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
    }
}
