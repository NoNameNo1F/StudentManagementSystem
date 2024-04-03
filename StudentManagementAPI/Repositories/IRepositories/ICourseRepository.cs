using StudentManagementAPI.Models;

namespace StudentManagementAPI.Repositories.IRepositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course> UpdateAsync(Course course);
    }
}
