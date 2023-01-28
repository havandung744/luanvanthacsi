using luanvanthacsi.Data.Entities;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using System.Linq;

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
    }
}
