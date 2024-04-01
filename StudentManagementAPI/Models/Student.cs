using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementAPI.Models
{
    public class Student : Person
    {
        [Required]
        [Column(TypeName = "char")]
        [StringLength(10)]
        public string? StudentId {get; set;}
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime YearEntrance {get; set;}
    }
}
