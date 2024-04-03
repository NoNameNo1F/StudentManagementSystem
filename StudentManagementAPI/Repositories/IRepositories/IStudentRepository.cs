using StudentManagementAPI.Models;

namespace StudentManagementAPI.Repositories.IRepositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student> UpdateAsync(Student student);
    }
}
