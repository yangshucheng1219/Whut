using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Models
{
    [Table("Enrollments")]
    public class Enrollment
    {
        public Enrollment()
        {
        }

        public Enrollment(int enrollmentID, int courseID, int strudentID)
        {
            EnrollmentID = enrollmentID;
            CourseID = courseID;
            StrudentID = strudentID;
        }
        [Key]
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int StrudentID { get; set; }
        public enum Grade { A,B,C,D,E }

    }
}
