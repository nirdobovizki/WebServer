using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public interface IHttpResponse
    {
        HttpStatusCode StatusCode { get; set; }
        long ContentLength64 { get; set; }
        Encoding ContentEncoding { get; set; }
        Stream OutputStream { get; }
        NameValueCollection Headers { get; }

        void End();
    }
}
