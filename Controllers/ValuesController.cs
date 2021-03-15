using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whut.Models;


namespace Whut.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly MySQLDbcontext _context;


        public ValuesController(MySQLDbcontext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public ActionResult<Student> Get(int id)
        {
            Student s = _context.students.Find(id);
            return s;
        }
    }
}
