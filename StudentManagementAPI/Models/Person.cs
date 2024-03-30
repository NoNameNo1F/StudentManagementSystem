using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using StudentManagementAPI.Enums;

namespace StudentManagementAPI.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }
        public eGender Gender { get; set; }

        [Column(TypeName = "char")]
		[StringLength(10)]
        public string? Contact { get; set; }
    }
}
