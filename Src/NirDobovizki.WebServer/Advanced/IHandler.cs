using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public interface IHandler
    {
        object PrepareHandlerData(object handlerParams, List<string> paramNames);
        Task ProcessRequest(IHttpContext context, List<string> paramValues, object handlerData);
    }
}
