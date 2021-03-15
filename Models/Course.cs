using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Models
{
    [Table("Courses")]
    public class Course
    {
        public Course()
        {
        }

        public Course(int courseID, string title, int credits, string finalWork, string url)
        {
            CourseID = courseID;
            Title = title;
            Credits = credits;
            FinalWork = finalWork;
            this.url = url;
        }
        [Key]
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public string FinalWork { get; set; }
        public string url { get; set; }
    }
}
