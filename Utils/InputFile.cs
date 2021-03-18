using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Utils
{
    public static class InputFile
    {
        public static List<string> inputFile(IFormFileCollection file,string courseid)
        {

            List<string> fileList = new List<string>();

            if (file.Count == 0)
            {
                fileList[0] = "null";
                return fileList;
            }

            string filePath = "C:\\Users\\Administrator\\source\\repos\\Whut\\images\\" + "Course"+ courseid + "\\";
            
            try
            {
                foreach (var item in file)
                {
                    string keyword = item.FileName;

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    using (FileStream fs = System.IO.File.Create(filePath + keyword)) {
                        //将item文件复制到fs文件
                        item.CopyTo(fs);
                        //清空缓冲区数据
                        fs.Flush();
                    }

                    String url = filePath + keyword;
                    fileList.Add(url);
                }
            }
            catch (Exception )
            {
                fileList[0] = "error";
                return fileList;
            }
            return fileList;
        }
        

    }
}
