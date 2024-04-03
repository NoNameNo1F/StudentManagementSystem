using AutoMapper;
using StudentManagementAPI.Models.Dtos;
using StudentManagementAPI.Models.Dtos.StudentDto;
using StudentManagementAPI.Models.Dtos.CourseDto;

namespace StudentManagementAPI.Models.StudentMSMapper
{
    public class StudentMSMappings : Profile{
        public StudentMSMappings()
        {
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, StudentCreateDto>().ReverseMap();
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<Course, CourseCreateDto>().ReverseMap();
        }
    }
}
