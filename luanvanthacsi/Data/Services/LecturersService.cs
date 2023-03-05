using luanvanthacsi.Data.Entities;
using ISession = NHibernate.ISession;
using NHibernate;
using NHibernate.Linq;
using MathNet.Numerics.Distributions;

namespace luanvanthacsi.Data.Services
{
    public class LecturersService : ILecturersService
    {
        public async Task<List<Lecturers>> GetAllByIdAsync(string id)
        {
            try
            {
                List<Lecturers> lectureres;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    lectureres = session.Query<Lecturers>().Where(x => x.FacultyId == id).ToList();
                }
                return lectureres;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
