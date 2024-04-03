using System.Linq.Expressions;
using StudentManagementAPI.Datas;
using StudentManagementAPI.Models;
using StudentManagementAPI.Repositories.IRepositories;

namespace StudentManagementAPI.Repositories
{
    public class CourseRepository : Repository<Course> , ICourseRepository
    {
        private readonly ApplicationDatabaseContext _db;
        public CourseRepository(ApplicationDatabaseContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            _db.Courses.Update(course);
            await _db.SaveChangesAsync();
            return course;
        }

    }
}
