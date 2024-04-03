using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using StudentManagementAPI.Enums;

namespace StudentManagementAPI.Models.Dtos.CourseDto
{
    public class CourseCreateDto
    {
        [Required]
        public string? Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }
    }
}
