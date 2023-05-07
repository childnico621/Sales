using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Shared.Responses
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; }
        public T? Result { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
