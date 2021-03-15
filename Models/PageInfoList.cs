using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Models
{
    public class PageInfoList
    {
        public PageInfoList()
        {
        }

        public PageInfoList(int count, int pageIndex, int pageSize, object item)
        {
            this.count = count;
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
            this.items = item;
        }


        public int count { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public object items { get; set; }
    }
}
