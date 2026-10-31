using System;
using System.Net;

namespace CineTec.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public object? Details { get; }

        public BusinessException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, object? details = null)
            : base(message)
        {
            StatusCode = statusCode;
            Details = details;
        }
    }
}
