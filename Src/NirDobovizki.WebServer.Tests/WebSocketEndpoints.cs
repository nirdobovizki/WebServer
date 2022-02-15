using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Tests
{


    [TestClass]
    public class WebSocketEndpoints
    {

        [TestMethod]
        public async Task HappyFlow()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            var srbuf = new byte[3];
            builder.ExposeWebSocketEndpoint("/widgets/{id}", async (int id, WebSocket sock) =>
            {
                await sock.ReceiveAsync(new Memory<byte>(srbuf), CancellationToken.None);
                await sock.SendAsync(new Memory<byte>(new byte[] { 4, 5, 6 }), WebSocketMessageType.Binary, true, CancellationToken.None);
                await sock.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            });
            using (var server = builder.BuildAndStart())
            {
                var client = new ClientWebSocket();
                await client.ConnectAsync(new Uri($"ws://localhost:{TestSettings.HttpPort}/widgets/8"), CancellationToken.None);
                var sbuf = new ReadOnlyMemory<byte>(new byte[] { 1, 2, 3 });
                await client.SendAsync(sbuf, WebSocketMessageType.Binary, true, CancellationToken.None);
                var crbuf = new byte[3];
                await client.ReceiveAsync(new Memory<byte>(crbuf), CancellationToken.None);
                Assert.IsTrue(srbuf[0] == 1 && srbuf[1] == 2 && srbuf[2] == 3);
                Assert.IsTrue(crbuf[0] == 4 && crbuf[1] == 5 && crbuf[2] == 6);

            }

        }

        [TestMethod]
        public async Task TwoWebSocketsAndGetAtTheSameTime()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            var srbuf = new byte[3];
            builder.ExposeWebSocketEndpoint("/widgets/{id}", async (int id, WebSocket sock) =>
            {
                await sock.ReceiveAsync(new Memory<byte>(srbuf), CancellationToken.None);
                await sock.SendAsync(new Memory<byte>(new byte[] { 4, 5, 6 }), WebSocketMessageType.Binary, true, CancellationToken.None);
                await sock.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            });
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/whatevet", () => "abc");
            using (var server = builder.BuildAndStart())
            {
                var client = new ClientWebSocket();
                var client2 = new ClientWebSocket();
                await client.ConnectAsync(new Uri($"ws://localhost:{TestSettings.HttpPort}/widgets/8"), CancellationToken.None);
                await client2.ConnectAsync(new Uri($"ws://localhost:{TestSettings.HttpPort}/widgets/8"), CancellationToken.None);
                using(var getClient = new HttpClient())
                {
                    await getClient.GetAsync($"http://localhost:{TestSettings.HttpPort}/whatever");
                }
                var sbuf = new ReadOnlyMemory<byte>(new byte[] { 1, 2, 3 });
                await client.SendAsync(sbuf, WebSocketMessageType.Binary, true, CancellationToken.None);
                await client2.SendAsync(sbuf, WebSocketMessageType.Binary, true, CancellationToken.None);
                var crbuf = new byte[3];
                await client.ReceiveAsync(new Memory<byte>(crbuf), CancellationToken.None);
                Assert.IsTrue(srbuf[0] == 1 && srbuf[1] == 2 && srbuf[2] == 3);
                Assert.IsTrue(crbuf[0] == 4 && crbuf[1] == 5 && crbuf[2] == 6);

            }

        }


        [TestMethod]
        public async Task NotWbSocketEndpoint()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets/{id}", (int id) =>
            {
                return "def";
            });
            using (var server = builder.BuildAndStart())
            {
                var client = new ClientWebSocket();
                try
                {
                    await client.ConnectAsync(new Uri($"ws://localhost:{TestSettings.HttpPort}/widgets/8"), CancellationToken.None);
                    Assert.Fail("Endpoint does not suppose to accept web sockets");
                }
                catch(WebSocketException)
                {
                    //good
                }
            }

        }


    }
}