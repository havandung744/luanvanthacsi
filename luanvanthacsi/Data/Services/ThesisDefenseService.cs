using luanvanthacsi.Data.Entities;
using MathNet.Numerics.Distributions;
using NHibernate;
using ISession = NHibernate.ISession;
using NHibernate.Linq;

namespace luanvanthacsi.Data.Services
{

    public class ThesisDefenseService : IThesisDefenseService
    {
        public async Task<bool> AddOrUpdateThesisDefenseAsync(ThesisDefense thesisDefense)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(thesisDefense.Id))
                        {
                            thesisDefense.Id = Guid.NewGuid().ToString();
                            thesisDefense.CreateDate = DateTime.Now;
                            await session.SaveAsync(thesisDefense);
                        }
                        else
                        {
                            await session.UpdateAsync(thesisDefense);
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

        public async Task<bool> DeleteThesisDefenseAsync(ThesisDefense thesisDefense)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        await session.DeleteAsync(thesisDefense);
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

        public async Task<bool> DeleteThesisDefenseListAsync(List<ThesisDefense> thesisDefenses)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in thesisDefenses)
                        {
                            await session.DeleteAsync(item);
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
            }
            return result;
        }

        public async Task<List<ThesisDefense>> GetAllAsync()
        {
            try
            {
                List<ThesisDefense> thesisDefenses;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    thesisDefenses = session.Query<ThesisDefense>().ToList();
                }
                return thesisDefenses;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<ThesisDefense>> GetAllByIdAsync(string id)
        {
            try
            {
                List<ThesisDefense> thesisDefense;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    thesisDefense = session.Query<ThesisDefense>().Where(x => x.FacultyId == id).ToList();
                }
                return thesisDefense;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Student>> GetCurrentListStaff(string FacultyId, string thesisDefensesId)
        {
            try
            {
                List<Student> students;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    students = session.Query<Student>().Where(c => (c.FacultyId == FacultyId) && (c.ThesisDefenseId == thesisDefensesId)).ToList();
                }
                return students;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<ThesisDefense>> GetListThesisDefenseBySearchAsync(string txtSearch)
        {
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        List<ThesisDefense> thesisDefenses;
                        thesisDefenses = session.Query<ThesisDefense>().Where(c => c.Name.Like('%' + txtSearch + '%')).ToList();
                        return thesisDefenses;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }
        }

        public async Task<ThesisDefense> GetThesisDefenseByIdAsync(string id)
        {
            ThesisDefense thesisDefense;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        thesisDefense = await session.GetAsync<ThesisDefense>(id);
                        return thesisDefense;
                    }
                    catch (Exception ex)
                    {
                        //await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }
        }

        public async Task<bool> UpdateStudentById(Student student)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        student.UpdateDate = DateTime.Now;
                        await session.UpdateAsync(student);

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

        public async Task<bool> UpdateStudentListByIds(List<Student> students)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var student in students)
                        {
                            student.UpdateDate = DateTime.Now;
                            await session.UpdateAsync(student);

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
