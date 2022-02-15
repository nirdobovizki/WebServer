using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.PubSub
{
    public abstract class BaseHub
    {
        private List<WebSocket> _subscribers = new List<WebSocket>();
        private SemaphoreSlim _lock = new SemaphoreSlim(1,1);

        public async Task AddSubscriber(WebSocket socket)
        {
            await _lock.WaitAsync();
            _subscribers.Add(socket);
            _lock.Release();
        }

        protected async Task PublishInternal(byte[] messageBytes,
            WebSocketMessageType messageType)
        {
            await _lock.WaitAsync();
            var subscribersBeforeSend = _subscribers.ToArray();
            try
            {
                var toSend = new ArraySegment<byte>(messageBytes);
                await Task.WhenAll(subscribersBeforeSend.Select(async (_, i) =>
                {
                    if (subscribersBeforeSend[i].State != WebSocketState.Open)
                    {
                        subscribersBeforeSend[i] = null;
                        return;
                    }
                    try
                    {
                        var timeout = new CancellationTokenSource();
                        timeout.CancelAfter(TimeSpan.FromSeconds(30));
                        await subscribersBeforeSend[i].
                            SendAsync(toSend, messageType, true, timeout.Token);
                    }
                    catch
                    {
                        subscribersBeforeSend[i] = null;
                    }
                }).ToArray());
            }
            finally
            {
                _subscribers = subscribersBeforeSend.Where(x => x != null).ToList();
                _lock.Release();
            }

        }
    }
}
