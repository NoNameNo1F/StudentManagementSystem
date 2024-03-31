using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using StudentManagementAPI.Enums;

namespace StudentManagementAPI.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }
    }
}
