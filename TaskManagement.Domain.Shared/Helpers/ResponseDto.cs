using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.Shared.Helpers
{
    public class ResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public int? StatusCode { get; set; }
        public string Message { get; set; }
    }
    public class ResponsePagedDto<T>: ResponseDto<T>
    {
        public Pagination Pagination { get; set; }
    }
    public class Pagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

}
