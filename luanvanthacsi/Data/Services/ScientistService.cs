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
using MathNet.Numerics.Distributions;

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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Scientist>> GetAllByIdAsync(string id)
        {
            try
            {
                List<Scientist> scientists;
                Specialized specialized;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    scientists = session.Query<Scientist>().Fetch(x => x.Specialized).Where(x => x.FacultyId == id).ToList();
                }
                return scientists;
            }
            catch (Exception)
            {
                throw;
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
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
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
                    catch (Exception)
                    {
                        //await transaction.RollbackAsync();
                        throw;
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
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> DeleteScientistListAsync(List<Scientist> scientists)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in scientists)
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
    }
}
