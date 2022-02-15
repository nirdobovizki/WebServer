using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public interface IHttpRequest
    {
        string HttpMethod { get; }
        Uri Url { get; }
        string UrlPath { get; set; }
        string InFolderPath { get; set; }
        Stream InputStream { get; }
        NameValueCollection QueryString { get; }
        NameValueCollection Headers { get; }
    }
}
