using StudentManagementAPI.Datas;
using StudentManagementAPI.Models;
using StudentManagementAPI.Repositories.IRepositories;

namespace StudentManagementAPI.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly CourseDatabaseContext _courseDbContext;
        public CourseRepository(CourseDatabaseContext courseDbContext)
        {
            _courseDbContext = courseDbContext;
        }
        public ICollection<Course> GetCourses()
        {
            return _courseDbContext.Courses.OrderBy(x => x.Id).ToList();
        }
        public Course GetCourse(int id)
        {
            return _courseDbContext.Courses.FirstOrDefault(x => x.Id == id);
        }

        public bool ExistsCourseById(int id)
        {
            return _courseDbContext.Courses.Any(x => x.Id == id);
        }

        public bool ExistsCourseByName(string name)
        {
            return _courseDbContext.Courses.Any(x => x.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool CreateCourse(Course course)
        {
            _courseDbContext.Courses.Add(course);
            return Save();
        }

        public bool UpdateCourse(Course course)
        {
            _courseDbContext.Courses.Update(course);
            return Save();
        }

        public bool DeleteCourse(Course course)
        {
            _courseDbContext.Courses.Remove(course);
            return Save();
        }

        public bool Save()
        {
            return _courseDbContext.SaveChanges() >= 0 ? true : false;
        }
    }
}
