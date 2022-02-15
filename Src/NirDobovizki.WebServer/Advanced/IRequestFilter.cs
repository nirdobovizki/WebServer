using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public interface IRequestFilter
    {
        Task<bool> BeforeParseRequest(IHttpContext context);
        Task<bool> BeforeExecuteHandler(IHttpContext context);
        Task AfterExecuteHandlerSuccessfully(IHttpContext context);
        void ExceptionDuringProcessing(IHttpContext context, Exception exception);




    }
}
