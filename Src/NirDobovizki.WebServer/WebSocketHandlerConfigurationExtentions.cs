using System;
using System.Collections.Generic;
using System.Text;

namespace NirDobovizki.WebServer
{
    public static class WebSocketHandlerConfigurationExtentions
    {
        private static Advanced.WebSocketEndpointHandler _handler = new Advanced.WebSocketEndpointHandler();
        public static void ExposeWebSocketEndpoint<P1>(this WebServerBuilder builder, string virtualPath, Action<P1> callback)
        {
            ExposeWebSocketEndpointInternal(builder, virtualPath, callback);
        }
        public static void ExposeWebSocketEndpoint<P1, P2>(this WebServerBuilder builder, string virtualPath, Action<P1, P2> callback)
        {
            ExposeWebSocketEndpointInternal(builder, virtualPath, callback);
        }
        public static void ExposeWebSocketEndpoint<P1, P2, P3>(this WebServerBuilder builder, string virtualPath, Action<P1, P2, P3> callback)
        {
            ExposeWebSocketEndpointInternal(builder, virtualPath, callback);
        }
        private static void ExposeWebSocketEndpointInternal(WebServerBuilder builder, string virtualPath, Delegate callback)
        {
            builder.Advanced.AddPathHandler(WebServerHttpMethod.GET, virtualPath, _handler, callback);
        }
    }
}
