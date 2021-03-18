using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Utils
{
    public class DeleteFile
    {
        public static string deleteFileByPath(string path) {
            DirectoryInfo dir = new DirectoryInfo(path);
            string message = "进入删除操作";
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
                dir.Delete(true);
                message = "删除成功";
            }
            else
            {
                message = "文件不存在";
            }
            return message;
        }
    }
}
