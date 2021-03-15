using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whut.Models;
using Whut.Utils;

namespace Whut.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("any")]
    public class StudentsController : ControllerBase
    {
        private readonly MySQLDbcontext _context;
        private readonly ICookieHelper _helper;

        public StudentsController (MySQLDbcontext context,ICookieHelper helper) {
            _context = context;
            _helper = helper;
        }

        private bool StudentExits(int id)
        {
            return _context.students.Any(e => e.StudentID == id);
        }

        private bool StudentNameExits(string name)
        {
            return _context.students.Any(e => e.Name == name);
        }
        private bool StudentDepartmentExits(string dep)
        {
            return _context.students.Any(e => e.Department == dep);
        }
        private bool StudentPassword(string pwd)
        {
            return _context.students.Any(e => e.Password == pwd);
        }

        [HttpGet("{id}")]
        public ActionResult<Student> Get(int id) {
            return _context.students.Find(id);
        }
        [HttpPost("Login")]
        public JsonResult Login(Student student){
            ResultState resultState = new ResultState();
            if (!StudentNameExits(student.Name))
            {
                resultState.Message = "该用户不存在";
                return new JsonResult(resultState);
            }
            var stuInDb = _context.students.Where(x => 
                            x.StudentID == student.StudentID)
                            .FirstOrDefault();
            if (student.Name != stuInDb.Name)
            {
                resultState.Message = "用户名错误";
                return new JsonResult(resultState);
            }
            if (student.Password == stuInDb.Password)
            {
                resultState.Success = true;
                if (stuInDb.isAdmin == 1)
                {
                    resultState.Code = 2;//Code=2表示学生管理员
                    resultState.Message = "学生管理员登录成功";
                }
                else
                {
                    resultState.Code = 1;//Code=1表示非学生管理员登陆成功
                    resultState.Message = "学生登录成功";
                }
                resultState.value = stuInDb;
                _helper.SetCookie("token", 
                    stuInDb.StudentID + "," + 
                    stuInDb.Name + "," + 
                    stuInDb.Enrollment + "," + 
                    stuInDb.Department + "," +
                    stuInDb.isAdmin);
            }
            return new JsonResult(resultState);
        }
        [HttpGet("StudentLogout")]
        public JsonResult StudentLogout() {
            var resultState = new ResultState();
            _helper.DeleteCookie("token");
            resultState.Success = true;
            resultState.Message = "注销成功";
            return new JsonResult(resultState);
        }

        [HttpPost("AddStudent")]
        public JsonResult AddStudent(Student student) {
            ResultState resultState = CheckCookie();
            if (resultState.Code == 2)
            {
                if (StudentExits(student.StudentID))
                {
                    resultState.Message = "学生ID已存在，添加失败";
                    resultState.Success = false;
                    return new JsonResult(resultState);
                }
                else
                {
                    _context.students.Add(student);
                    _context.SaveChangesAsync();
                    resultState.Message = "添加学生成功";
                    resultState.Success = true;
                    resultState.value = student;
                }
            }
            else {
                resultState.Message = "您的权限被限制，请联系管理员获得权限";
                resultState.Success = false;
            }
            return new JsonResult(resultState);
        }

        [HttpDelete("DeleteStudent")]
        public async Task<ActionResult> DeleteStudent(Student student) 
        {
            ResultState resultState = CheckCookie();
            if (resultState.Code == 2)
            {
                var stuInDb = await _context.students.FindAsync(student.StudentID);

                if (student.StudentID != stuInDb.StudentID ||
                    student.Password != stuInDb.Password ||
                    student.Name != stuInDb.Name ||
                    student.isAdmin != stuInDb.isAdmin
                    ){
                    resultState.Message = "删除失败，请重新检查账户信息";
                    resultState.Success = false;
                }
                else
                {
                    _context.students.Remove(stuInDb);
                    await _context.SaveChangesAsync();
                    if (!StudentExits(student.StudentID))
                    {
                        resultState.Success = true;
                        resultState.Message = "成功删除";
                        return new JsonResult(resultState);
                    }
                }
            }
            return new JsonResult(resultState);
        }

        [HttpPut("UpdateStudentInfo")]
        public JsonResult UpdateStudentInfo(Student student)
        {
            ResultState resultState = CheckCookie();
            //ResultState resultState = new ResultState();
           // resultState.Code = 1;
            if (resultState.Code != 0)
            {
                var stuInDb = _context.students.Find(student.StudentID);
                if (stuInDb.Name == student.Name &&
                    stuInDb.Password == student.Password &&
                    stuInDb.StudentID == student.StudentID &&
                    stuInDb.isAdmin == student.isAdmin)
                {
                    resultState.Message = "未做任何修改";
                    return new JsonResult(resultState);
                }
                else if (stuInDb.Name != student.Name)
                {
                    resultState.Message = "学生名不可修改，请重新设定";
                    resultState.value = stuInDb;
                    return new JsonResult(resultState);
                }
                stuInDb.Enrollment = student.Enrollment;
                stuInDb.Department = student.Department;
                stuInDb.Password = student.Password;
                stuInDb.isAdmin = student.isAdmin;
                _context.SaveChanges();
                resultState.Message = "修改成功";
                resultState.Success = true;
                resultState.value = student;
            }
            return new JsonResult(resultState);
        }

        [HttpPost("StudentInfoList")]
        public JsonResult StudentInfoList([FromBody] QueryParameters query) 
        {
            ResultState resultState = CheckCookie();
            if (resultState.Code == 2)
            {
                int count = _context.students.Count();
                List<Student> studentsInCurrentList = new List<Student>();
                //初始化用户表
                PageInfoList pageStudents = new PageInfoList();
                pageStudents.items = studentsInCurrentList;
                pageStudents.count = count;
                pageStudents.pageIndex = query.PageIndex;
                pageStudents.pageSize = query.PageSize;
                //查询
                if (query.PageIndex <= 0)
                {
                    studentsInCurrentList = _context.students.Take(query.PageSize).ToList();
                    pageStudents.items = studentsInCurrentList;
                    pageStudents.pageIndex = 1;
                }
                else if ((query.PageSize * query.PageIndex) <= count)
                {
                    studentsInCurrentList = _context.students.Skip(query.PageSize * (query.PageIndex - 1))
                        .Take(query.PageSize).ToList();
                    pageStudents.items = studentsInCurrentList;
                }
                else if ((query.PageSize * query.PageIndex) > count && count % query.PageSize == 0)
                {
                    studentsInCurrentList = _context.students.Skip(count - query.PageSize).Take(query.PageSize).ToList();
                    pageStudents.items = studentsInCurrentList;
                    //count超过最后一页，count / query.PageSize可以整除的情况
                    pageStudents.pageIndex = count / query.PageSize;
                }
                else {
                    studentsInCurrentList = _context.students.Skip(count - count % query.PageSize).Take(count % query.PageSize).ToList();
                    pageStudents.items = studentsInCurrentList;
                    //count超过最后一页，pageIndex为最后一页count / query.PageSize一定不是整数
                    pageStudents.pageIndex = (count / query.PageSize) + 1;
                }
                resultState.Success = true;
                resultState.Message = "查询成功";
                resultState.value = pageStudents;
            }
            return new JsonResult(resultState);
        }


        private ResultState CheckCookie()
        {
            string getToken = _helper.GetCookie("token");
            if(getToken == null)
            {
                return new ResultState(false, "请登录", 0, null);
            }
            var devidedToken = getToken.Split(",");
            try
            {
                var student = _context.students.Find(int.Parse(devidedToken[0]));
                if (student != null)
                {
                    int isAdmin = student.isAdmin + 1;
                    return new ResultState(true, "验证成功", isAdmin, null);

                }
                else
                {
                    return new ResultState(true, "无效Cookie", 0, null);
                }
            }
            catch (Exception)
            {
                return new ResultState(false, "无效Cookie", 0, null);
            }
        }




    }
}
