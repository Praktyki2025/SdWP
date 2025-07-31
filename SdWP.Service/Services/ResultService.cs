using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Service.Services
{
    public class ResultService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ResultService<T> GoodResult(
            T data, 
            string message, 
            int statusCode)
            => new ResultService<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };

        public static ResultService<T> BadResult(
            T data,
            string message,
            int statusCode,
            List<string>? errors = null)
            => 
            new ResultService<T>
            {
                Success = false,
                Data = data,
                Message = message,
                StatusCode = statusCode,
                Errors = errors ?? new List<string>()
            };
    }
}

