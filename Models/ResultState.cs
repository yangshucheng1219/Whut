using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whut.Models
{
    public class ResultState
    {
        public ResultState()
        {
        }

        public ResultState(bool success, string message,  int code, object value)
        {
            Success = success;
            Message = message;
            Code = code;
            this.value = value;
        }

        public int Code { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public Object value { get; set; }

    }
}
