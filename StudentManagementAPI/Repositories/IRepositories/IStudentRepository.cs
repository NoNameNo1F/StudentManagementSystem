using StudentManagementAPI.Models;

namespace StudentManagementAPI.Repositories.IRepositories
{
    public interface IStudentRepository
    {
        ICollection<Student> GetStudents();

        Student GetStudent(int studentId);

        bool ExistsStudentById(int studentId);
        bool ExistsStudentByStudentId(string studentStringId);
        bool ExistsStudentByName(string studentName);

        bool CreateStudent(Student student);

        bool UpdateStudent(Student student);

        bool DeleteStudent(Student student);

        bool Save();
    }
}
