using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Models
{
    [Table("Students")]
    public class Student
    {
        public Student()
        {
        }

        public Student(int studentID, string name, DateTime enrollment, string department, string password)
        {
            StudentID = studentID;
            Name = name;
            Enrollment = enrollment;
            Department = department;
            Password = password;
        }

        [Key]
        public int StudentID { get; set; }
        public string Name { get; set; }
        public DateTime Enrollment { get; set; }
        public string Department { get; set; }
        public string Password { get; set; }
        public int isAdmin { get; set; }
    }
}
