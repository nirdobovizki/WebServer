using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public class WebSocketEndpointHandler : LambdaEndpointHandler
    {
        protected override Task<object> TransformBodyToObject(Stream inputStream, Type expectedType)
        {
            throw new NotImplementedException();
        }

        protected override Task TransformResultIntoStream(object result, IHttpContext context)
        {
            throw new NotImplementedException();
        }

        public WebSocketEndpointHandler()
        {
            AddTypedParameter(typeof(WebSocket), ctxt => ctxt.UpgradeToWebSocket());
        }
    }
}
