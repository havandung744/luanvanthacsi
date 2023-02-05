using luanvanthacsi.Data.Entities;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
                            scientist.CreateDate = DateTime.Now;
                            scientist.UpdateDate = DateTime.Now;
                            await session.SaveAsync(scientist);
                        }
                        else
                        {
                            scientist.UpdateDate = DateTime.Now;
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

        public async Task<Scientist> GetScientistByIdAsync(string id)
        {
            Scientist scientist;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        scientist = await session.GetAsync<Scientist>(id);
                        return scientist;
                    }
                    catch (Exception ex)
                    {
                        //await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }
        }

        public async Task<bool> DeleteScientistAsync(Scientist scientist)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {

                        await session.DeleteAsync(scientist);
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

        public async Task<List<Scientist>> GetListScientistBySearchAsync(string txtSearch)
        {
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        List<Scientist> scientists;
                        scientists = session.Query<Scientist>().Where(c => c.Name.Like('%' + txtSearch + '%')).ToList();
                        //await transaction.CommitAsync();
                        return scientists;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }
        }
    }
}
