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
    public class FacultyService : IFacultyService
    {
        public async Task<List<Faculty>> GetAllAsync()
        {
            try
            {
                List<Faculty> faculties;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    faculties = session.Query<Faculty>().ToList();
                }
                return faculties;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
