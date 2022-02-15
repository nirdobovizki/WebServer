using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.PubSub
{
    public class JsonHub : BaseHub
    {
        Advanced.IJsonSerializer _serializer;

        public JsonHub(Advanced.IJsonSerializer serializer = null)
        {
            _serializer = serializer ?? new Advanced.DefaultJsonSerializer();
        }

        public Task Publish(object obj)
        {
            var str = _serializer.Serialize(obj);
            return PublishInternal(Encoding.UTF8.GetBytes(str),
                System.Net.WebSockets.WebSocketMessageType.Text);
        }
    }
}
