using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer
{
    public class WebServerException : Exception
    {
        public readonly HttpStatusCode StatusCode;
       
        public WebServerException(HttpStatusCode statusCode, string message):
            base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : WebServerException
    {
        public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
    }

    public class BadRequestException : WebServerException
    {
        public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }
    }

    public class ForbiddenException : WebServerException
    {
        public ForbiddenException(string message) : base(HttpStatusCode.Forbidden, message) { }
    }
}
