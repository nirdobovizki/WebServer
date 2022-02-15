using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public class NotFoundHandler : IHandler
    {
        public object PrepareHandlerData(object handlerParams, List<string> paramNames)
        {
            return null;
        }

        public Task ProcessRequest(IHttpContext context, List<string> paramValues, object handlerData)
        {
            context.Response.StatusCode = HttpStatusCode.NotFound;
            using (var writer = new StreamWriter(context.Response.OutputStream))
            {
                writer.WriteLine("Path Not Found");
            }
            return Task.CompletedTask;
        }
    }
}
