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
    public class StudentService : IStudentService
    {
        readonly IMapper _mapper;
        public StudentService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<bool> AddListStudentAsync(List<Student> students)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (Student student in students)
                        {
                            student.CreateDate = DateTime.Now;
                            student.UpdateDate = DateTime.Now;
                            await session.SaveAsync(student);
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
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                    }
                }
                return result;
            }

        }

        public async Task<bool> AddOrUpdateStudentListAsync(List<Student> students, string facultyId)
        {
            bool result = false;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        IQueryable<Student> Query = session.Query<Student>().Where(x => x.FacultyId == facultyId);
                        foreach (Student student in students)
                        {
                            var exist = Query.FirstOrDefault(c => c.Code == student.Code);
                            if (exist != null)
                            {
                                if (student.DateOfBirth == DateTime.MinValue)
                                {
                                    student.DateOfBirth = null;
                                }
                                exist.UpdateDate = DateTime.Now;
                                exist.Code = student.Code;
                                exist.Name = student.Name;
                                exist.DateOfBirth = student.DateOfBirth;
                                exist.Email = student.Email;
                                exist.PhoneNumber = student.PhoneNumber;
                                exist.TopicName = student.TopicName;
                                exist.SpecializedId = student.SpecializedId;    
                                exist.InstructorIdOne = student.InstructorIdOne;
                                exist.InstructorIdTwo = student.InstructorIdTwo;
                                await session.MergeAsync(exist);
                            }
                            else
                            {
                                student.CreateDate = DateTime.Now;
                                student.UpdateDate = DateTime.Now;
                                if (student.DateOfBirth == DateTime.MinValue)
                                {
                                    student.DateOfBirth = null;
                                }
                                await session.SaveAsync(student);
                            }
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
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
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
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
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
