using AutoMapper;
using StudentManagementAPI.Models.Dtos;
using StudentManagementAPI.Models.Dtos.StudentDto;

namespace StudentManagementAPI.Models.StudentMSMapper
{
    public class StudentMSMappings : Profile{
        public StudentMSMappings()
        {
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, StudentCreateDto>().ReverseMap();
        }
    }
}
