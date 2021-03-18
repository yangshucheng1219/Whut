using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Utils
{
    public class UploadFile
    {
        public List<string> uploadFile(IFormFileCollection file) {
            List<string> fileList = new List<String>();
            if (file.Count == 0)
            {
                fileList.Add("null");
                return fileList;
            }

            try
            {
                foreach (var item in file)
                {
                    string keyword = item.FileName;

                    string filePath = "C:\\Users\\Administrator\\source\\repos\\Whut\\images";

                    //如果filePath不存在，则创建它
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    //创建文件
                    using (FileStream fs = File.Create(filePath + keyword)) {
                        //复制文件
                        item.CopyTo(fs);
                        //清空缓冲区的数据
                        fs.Flush();
                    }

                    String url = filePath + keyword;

                    fileList.Add(url);
                }
            }
            catch (Exception)
            {
                fileList.Add("error");
                return fileList;
            }
            return fileList;
        }
    }
}
