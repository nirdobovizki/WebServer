using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public interface IAdvancedWebServerBuilder
    {
        void AddPathHandler(WebServerHttpMethod method, string path, IHandler handler, object handlerParameter);
        Func<int, int, bool, IHttpEngine> HttpEngineFactory { get; set; }
    }
}
