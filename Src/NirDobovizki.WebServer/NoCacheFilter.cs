using NirDobovizki.WebServer.Advanced;
using System;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer
{
    public class NoCacheFilter : IRequestFilter
    {
        public Task AfterExecuteHandlerSuccessfully(IHttpContext context)
        {
            return Task.FromResult(true);
        }

        public Task<bool> BeforeExecuteHandler(IHttpContext context)
        {
            if(context.Response.Headers["Cache-Control"] == null)
            {
                context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            }
            return Task.FromResult(true);
        }

        public Task<bool> BeforeParseRequest(IHttpContext context)
        {
            return Task.FromResult(true);
        }

        public void ExceptionDuringProcessing(IHttpContext context, Exception exception)
        {
        }
    }
}