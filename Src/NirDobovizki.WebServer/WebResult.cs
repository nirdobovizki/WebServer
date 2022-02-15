using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NirDobovizki.WebServer
{
    public class WebResult
    {
        public readonly HttpStatusCode Status;
        public readonly object Data;
        public WebResult(HttpStatusCode status, object data)
        {
            Status = status;
            Data = data;
        }
    }

    public class WebResult<T> : WebResult
    {
        public readonly new T Data;
        public WebResult(HttpStatusCode status, T data):base(status,data)
        {
            Data = data;
        }
    }
}
