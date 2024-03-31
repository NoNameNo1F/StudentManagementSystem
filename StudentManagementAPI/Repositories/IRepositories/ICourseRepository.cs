using StudentManagementAPI.Models;

namespace StudentManagementAPI.Repositories.IRepositories
{
    public interface ICourseRepository
    {
        ICollection<Course> GetCourses();
        Course GetCourse(int id);
        bool ExistsCourseById(int id);
        bool ExistsCourseByName(string name);
        bool CreateCourse(Course course);
        bool UpdateCourse(Course course);
        bool DeleteCourse(Course course);
        bool Save();
    }
}
