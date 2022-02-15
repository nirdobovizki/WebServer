using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using NirDobovizki.WebServer.PubSub;
using System.Collections.Generic;
using System.Text;

namespace NirDobovizki.WebServer.Tests
{


    [TestClass]
    public class PubSubHub
    {

 
        [TestMethod]
        public async Task HubPublish()
        {
            var timeout = new CancellationTokenSource();
            timeout.CancelAfter(TimeSpan.FromSeconds(30));
            int okCount = 0;
            int failCount = 0;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            var hub = new JsonHub();
            builder.ExposePubSubSubscribeEndpoint("/notifications", hub);
            using (var server = builder.BuildAndStart())
            {
                var threads = new List<Task>();
                for(int i=0;i<3;++i)
                {
                    threads.Add(Task.Run(async() =>
                    {
                        var client = new ClientWebSocket();
                        await client.ConnectAsync(new Uri($"ws://localhost:{TestSettings.HttpPort}/notifications"), CancellationToken.None);
                        var buffer = new byte[1000];
                        var rec = await client.ReceiveAsync(buffer, timeout.Token);
                        var str = Encoding.UTF8.GetString(buffer, 0, rec.Count);
                        if(str == "\"abc\"")
                        {
                            Interlocked.Increment(ref okCount);
                        }
                        else
                        {
                            Interlocked.Increment(ref failCount);
                        }
                    }));
                }
                await Task.Delay(1000);
                await hub.Publish("abc");
                await Task.WhenAll(threads.ToArray());
                Assert.AreEqual(3, okCount);
                Assert.AreEqual(0, failCount);
            }
        }

        [TestMethod]
        public async Task ClientDisconnect()
        {
            var timeout = new CancellationTokenSource();
            timeout.CancelAfter(TimeSpan.FromSeconds(30));

            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            var hub = new JsonHub();
            builder.ExposePubSubSubscribeEndpoint("/notifications", hub);
            using (var server = builder.BuildAndStart())
            {
                var threads = new List<Task>();
                for (int i = 0; i < 3; ++i)
                {
                    threads.Add(Task.Run(async () =>
                    {
                        var client = new ClientWebSocket();
                        await client.ConnectAsync(new Uri($"ws://localhost:{TestSettings.HttpPort}/notifications"), CancellationToken.None);
                        client.Dispose();
                    }));
                }
                await Task.WhenAll(threads.ToArray());
                await hub.Publish("abc");
            }
        }




    }
}