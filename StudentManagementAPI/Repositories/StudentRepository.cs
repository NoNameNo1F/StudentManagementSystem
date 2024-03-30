using StudentManagementAPI.Models;
using StudentManagementAPI.Repositories.IRepositories;
using StudentManagementAPI.Datas;

namespace StudentManagementAPI.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentDatabaseContext _studentDbContext;
        public StudentRepository(StudentDatabaseContext studentDbContext)
        {
            _studentDbContext = studentDbContext;
        }
        public ICollection<Student> GetStudents()
        {
            return _studentDbContext.Students.OrderBy(x => x.StudentId).ToList();
        }

        public Student GetStudent(int studentId)
        {
            return _studentDbContext.Students.FirstOrDefault(x => x.Id == studentId);
        }

        public bool ExistsStudentById(int studentId)
        {
            return _studentDbContext.Students.Any(x => x.Id == studentId);
        }
        public bool ExistsStudentByStudentId(string studentStringId)
        {
            return _studentDbContext.Students.Any(x => x.StudentId.ToLower() == studentStringId.ToLower());
        }
        public bool ExistsStudentByName(string studentName)
        {
            return _studentDbContext.Students.Any(x => x.FirstName.ToLower().Trim() == studentName.ToLower().Trim());
        }

        public bool CreateStudent(Student student)
        {
            _studentDbContext.Students.Add(student);
            return Save();
        }

        public bool DeleteStudent(Student student)
        {
            _studentDbContext.Students.Remove(student);
            return Save();
        }

        public bool UpdateStudent(Student student)
        {
            _studentDbContext.Students.Update(student);
            return Save();
        }

        public bool Save()
        {
            return _studentDbContext.SaveChanges() >= 0 ? true : false;
        }
    }
}
