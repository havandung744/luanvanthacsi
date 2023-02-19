using luanvanthacsi.Data.Entities;
using ISession = NHibernate.ISession;
using NHibernate;
using NHibernate.Linq;
using MathNet.Numerics.Distributions;

namespace luanvanthacsi.Data.Services
{
    public class StudentService : IStudentService
    {
        public async Task<bool> AddOrUpdateStudentAsync(Student student)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(student.Id))
                        {
                            student.Id = Guid.NewGuid().ToString();
                            student.CreateDate = DateTime.Now;
                            student.UpdateDate = DateTime.Now;
                            await session.SaveAsync(student);
                        }
                        else
                        {
                            Student studentDb = await GetStudentByIdAsync(student.Id);
                            string thesisDefenseId = studentDb?.ThesisDefenseId;
                            student.UpdateDate = DateTime.Now;
                            student.ThesisDefenseId = thesisDefenseId;
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

        public async Task<bool> DeleteStudentAsync(Student student)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {

                        await session.DeleteAsync(student);
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

        public async Task<bool> DeleteStudentListAsync(List<Student> students)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in students)
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

        public async Task<List<Student>> GetAllAsync()
        {
            try
            {
                List<Student> students;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    students = session.Query<Student>().ToList();
                }
                return students;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Student>> GetAllByIdAsync(string id)
        {
            try
            {
                List<Student> students;
                using (ISession session = FluentNHibernateHelper.OpenSession())
                {
                    students = session.Query<Student>().Where(x => x.FacultyId == id).ToList();
                }
                return students;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Student>> GetListStudentBySearchAsync(string txtSearch)
        {
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        List<Student> students;
                        students = session.Query<Student>().Where(c => c.Name.Like('%' + txtSearch + '%')).ToList();
                        return students;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }
        }

        public async Task<Student> GetStudentByIdAsync(string id)
        {
            Student student;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        student = await session.GetAsync<Student>(id);
                        return student;
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
