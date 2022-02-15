using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer
{
    public static class JsonApiHandlerConfigurationExtentions
    {
        private static Advanced.JsonApiHandler _handler = new Advanced.JsonApiHandler();
        public static void ExposeJsonEndpoint<R>(this WebServerBuilder builder, WebServerHttpMethod method, string virtualPath, Func<R> callback)
        {
            ExposeJsonEndpointInternal(builder, method, virtualPath, callback);
        }
        public static void ExposeJsonEndpoint<R, P1>(this WebServerBuilder builder, WebServerHttpMethod method, string virtualPath, Func<P1, R> callback)
        {
            ExposeJsonEndpointInternal(builder, method, virtualPath, callback);
        }
        public static void ExposeJsonEndpoint<R, P1, P2>(this WebServerBuilder builder, WebServerHttpMethod method, string virtualPath, Func<P1, P2, R> callback)
        {
            ExposeJsonEndpointInternal(builder, method, virtualPath, callback);
        }
        private static void ExposeJsonEndpointInternal(WebServerBuilder builder, WebServerHttpMethod method, string virtualPath, Delegate callback)
        {
            builder.Advanced.AddPathHandler(method, virtualPath, _handler, callback);
        }


    }
}
