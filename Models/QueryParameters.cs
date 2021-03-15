using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Models
{
    public class QueryParameters
    {
        public QueryParameters()
        {
        }

        public QueryParameters(int pageSize, int pageIndex)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
        }


        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
