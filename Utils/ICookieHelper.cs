using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Utils
{
    public interface ICookieHelper
    {
        void SetCookie(string key, string value);
        void SetCookie(string key, string value, int expiresTime);
        string GetCookie(string key);
        void DeleteCookie(string key);

    }
}
