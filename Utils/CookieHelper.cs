using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whut.Models;

namespace Whut.Utils
{
    public class CookieHelper : ICookieHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 通过构造函数进行注入
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public CookieHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 根据key值删除对应的Cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void DeleteCookie(string key)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 根据key值获取Cookie的value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetCookie(string key)
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[key];
        }


        /// <summary>
        /// 设置Cookie值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void SetCookie(string key, string value)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value);
        }


        /// <summary>
        /// 设置cookie值以及过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresTime"></param>
        public void SetCookie(string key, string value, int expiresTime)
        {
            CookieOptions options = new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(expiresTime)
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, options);
        }

       
    }
}
