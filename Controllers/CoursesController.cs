using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whut.Models;
using Whut.Utils;
using Whut.Controllers;
using Microsoft.AspNetCore.Cors;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Whut.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("any")]
    public class CoursesController : ControllerBase
    {
        private readonly MySQLDbcontext _context;
        private readonly ICookieHelper _helper;

        public CoursesController(MySQLDbcontext context, ICookieHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        private bool CourseExists(int courseid)
        {
            return _context.Courses.Any(x => x.CourseID == courseid);
        }

        [HttpPut("CourseList")]
        public async Task<JsonResult> CourseList([FromBody] QueryParameters query ) {
            PageInfoList pageInfoList = new PageInfoList();
            int count = _context.Courses.AsNoTracking().Count();
            pageInfoList.count = count;
            pageInfoList.pageIndex = query.PageIndex;
            pageInfoList.pageSize = query.PageSize;
            if (query.PageIndex <= 0)
            {
                var item = await _context.Courses.Take(query.PageSize).ToListAsync();
                pageInfoList.items = item;
                pageInfoList.pageIndex = 1;
            }
            else if (query.PageSize * query.PageIndex <= pageInfoList.count)
            {
                var item = await _context.Courses.AsNoTracking().Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToListAsync();
                pageInfoList.items = item;
            }
            else
            {
                if (count % query.PageSize == 0)
                {
                    var item = await _context.Courses.AsNoTracking().Skip(count - query.PageSize).Take(query.PageSize).ToListAsync();
                    pageInfoList.items = item;
                    pageInfoList.pageIndex = count / query.PageSize;
                }
                else
                {
                    var item = await _context.Courses.AsNoTracking().Skip(count - count % query.PageSize).Take(query.PageSize).ToListAsync();
                    pageInfoList.items = item;
                    pageInfoList.pageIndex = count / query.PageSize + 1;
                }
            }
            return new JsonResult(pageInfoList);
        }

        [HttpPut("UpdateCourse")]
        public async Task<JsonResult> UpdateCourse(Course course) {
            ResultState resultState = CheckCookie();
            if (resultState.Code == 2)
            {

                var couInDb = await _context.Courses.FindAsync(course);
                if (couInDb.CourseID == course.CourseID &&
                    couInDb.Credits == course.Credits &&
                    couInDb.FinalWork == course.FinalWork &&
                    couInDb.Title == course.Title &&
                    couInDb.url == course.url
                    )
                {
                    resultState.Success = false;
                    resultState.Message = "课程信息没有变化";
                    return new JsonResult(resultState);
                }
                else if (couInDb == null)
                {
                    resultState.Success = false;
                    resultState.Message = "无此课程信息";
                    return new JsonResult(resultState);
                }
                else {
                    _context.Courses.Update(course);
                    await _context.SaveChangesAsync();
                    resultState.Success = true;
                    resultState.Message = "课程信息更新成功";
                    return new JsonResult(resultState);
                }
            }
            return new JsonResult(resultState);
        }

        [HttpPost("AddCourse")]
        public async Task<IActionResult> AddCourse(Course course)
        {
            ResultState resultState = CheckCookie();
            if (resultState.Code == 2)
            {
                var couInDb = await _context.Courses.FindAsync(course.CourseID);
                if (couInDb != null)
                {
                    resultState.Message = "已存在此课程，请重新添加课程";
                    resultState.Success = false;
                    return new JsonResult(resultState);
                }
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                resultState.Message = "添加课程成功";
                resultState.Success = true;
            }
            return new JsonResult(resultState);
        }


        // GET: api/<CoursesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1" + System.Environment.NewLine + "value2" };
        }



        [HttpDelete("DeleteFilesByCourseID")]
        public async Task<IActionResult> DeleteFilesByCourseID(int courseId)
        {
            ResultState resultState = CheckCookie();
            if (resultState.Code == 2)
            {
                //var urls = _context.Courses.Where(f => f.CourseID == courseId).Select(f => f.url).FirstOrDefault().Split("|");
                var cousurlInD = await _context.Courses.FirstOrDefaultAsync(x => x.CourseID == courseId);
                var url = cousurlInD.url;
                var urls = url.Split("|");
                var delPath = urls[0];
                DirectoryInfo di = new DirectoryInfo(delPath);
                delPath = di.Parent.ToString();
                resultState.value = delPath + "         " + url;
                string message = DeleteFile.deleteFileByPath(delPath);
                resultState.Message = message;
                if (message == "删除成功")
                {
                    resultState.Success = true;

                    cousurlInD.url = null;
                    await _context.SaveChangesAsync();
                    return new JsonResult(resultState);
                }
                else
                {
                    resultState.Success = false;
                    return new JsonResult(resultState);
                }
            }
            resultState.Success = false;
            resultState.Message = "权限不够";
            return new JsonResult(resultState);
        }


        // POST api/<CoursesController>
        [HttpPost("AddFinalWork")]
        public JsonResult AddFinalWork([FromForm] IFormFileCollection WorkFile,
             [FromForm] int coursid)
        {
            var file = WorkFile;
            ResultState resultState = CheckCookie();
            //bool success, string message,  int code, object value
            //ResultState resultState = new ResultState(true, "", 2, null);
            if (resultState.Code != 2)
            {
                return new JsonResult(new ResultState(false, "权限不够或者没有登录", 0, null));
            }
            if (file == null)
            {
                return new JsonResult(new ResultState(false, "未添加文件", 0, null));
            }
            try
            {
                string couidString = coursid.ToString();
                List<string> url = InputFile.inputFile(file, couidString);

                if (url[0] == "null")
                {
                    return new JsonResult(new ResultState(false, "文件参数异常", 0, null));
                }
                else if (url[0] == "error")
                {
                    return new JsonResult(new ResultState(false, "上传失败", 0, null));
                }
                else
                {
                    var couseInDb = _context.Courses.Where(x => x.CourseID == coursid).FirstOrDefault();
                    StringBuilder courseurl = new StringBuilder();
                    courseurl.Append(couseInDb.url);
                    foreach (var urlname in url)
                    {
                        courseurl.Append(urlname);
                        courseurl.Append("|");
                    }
                    couseInDb.url = courseurl.ToString();
                    _context.SaveChanges();
                    return new JsonResult(new ResultState(true, "上传成功", 1, url));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        [HttpDelete("DeleteCourse")]
        public async Task<IActionResult> DeleteCourse(Course course)
        {
            ResultState resultState = CheckCookie();
            if (resultState.Code == 2)
            {
                if (CourseExists(course.CourseID))
                {

                    var url = _context.Courses.AsNoTracking().Where(f => f.CourseID == course.CourseID).Select(f => f.url).FirstOrDefault();
               
                    if (url != null)
                    {
                        var urls = url.Split("|");
                        var delPath = urls[0];
                        DirectoryInfo di = new DirectoryInfo(delPath);
                        delPath = di.Parent.ToString();
                        resultState.value = delPath + "         " + urls;
                        string message = DeleteFile.deleteFileByPath(delPath);
                        resultState.Message = message;
                        if (message == "删除成功")
                        {
                            _context.Courses.Remove(course);
                            await _context.SaveChangesAsync();
                            resultState.Message = "删除课程信息成功";
                            resultState.Success = true;
                            return new JsonResult(resultState);
                        }
                        else
                        {
                            resultState.Success = false;
                            return new JsonResult(resultState);
                        }

                    }
                    else
                    {
                        _context.Courses.Remove(course);
                        await _context.SaveChangesAsync();
                        resultState.Message = "删除课程信息成功,此课程无文件";
                        resultState.Success = true;
                        return new JsonResult(resultState);
                    }
                    
                }
                else
                {
                    resultState.Message = "删除课程失败";
                    resultState.Success = false;
                }

            }
            return new JsonResult(resultState);
        }

        // DELETE api/<CoursesController>/5



        private ResultState CheckCookie()
        {
            string getToken = _helper.GetCookie("token");
            if (getToken == null)
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
