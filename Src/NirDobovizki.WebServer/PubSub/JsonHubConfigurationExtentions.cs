using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace NirDobovizki.WebServer.PubSub
{
    public static class JsonHubConfigurationExtentions
    {
        public static void ExposePubSubSubscribeEndpoint(this WebServerBuilder builder, string virtualPath, BaseHub hub)
        {
            builder.ExposeWebSocketEndpoint(virtualPath, async (WebSocket socket) => await hub.AddSubscriber(socket));
        }
    }
}
