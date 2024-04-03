using StudentManagementAPI.Models;
using StudentManagementAPI.Repositories.IRepositories;
using StudentManagementAPI.Datas;

namespace StudentManagementAPI.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        private readonly ApplicationDatabaseContext _db;
        public StudentRepository(ApplicationDatabaseContext db) : base(db)
        {
            _db = db;
        }
        public async Task<Student> UpdateAsync(Student student)
        {
            _db.Students.Update(student);
            await _db.SaveChangesAsync();
            return student;
        }
    }
}
