using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Models
{
    public class MySQLDbcontext : DbContext
    {
        public MySQLDbcontext (DbContextOptions<MySQLDbcontext> options) : base(options)
        {

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

    }
}
