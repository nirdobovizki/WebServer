using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public abstract class TypedParameterHandler<TData,TParam> : IHandler
    {
        object IHandler.PrepareHandlerData(object handlerParams, List<string> paramNames)
        {
            return PrepareHandlerData((TParam)handlerParams, paramNames);
        }

        Task IHandler.ProcessRequest(IHttpContext context, List<string> paramValues, object handlerData)
        {
            return ProcessRequest(context, paramValues, (TData)handlerData);
        }

        protected abstract TData PrepareHandlerData(TParam handlerParams, List<string> paramNames);

        protected abstract Task ProcessRequest(IHttpContext context, List<string> paramValues, TData handlerData);

    }
}
