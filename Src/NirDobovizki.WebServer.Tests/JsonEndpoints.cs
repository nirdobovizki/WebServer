using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Tests
{

    [TestClass]
    public class JsonEndpoints
    {
        private class Data
        {
            public string? Name;
        }

        private HttpClient _client = new HttpClient();
        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
        }

        [TestMethod]
        public async Task PostJsonBodyAndReturnJson()
        {
            string? gotValue = null;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", (int id, Data body) =>
            {
                gotValue = body.Name;
                return new Data { Name = "def"};
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("{\"Name\":\"abc\"}"));
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual("abc", gotValue);
            }

        }

        [TestMethod]
        public async Task PostNonJsonBody()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", (int id, Data body) =>
            {
                return new Data { Name = "def" };
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, res.StatusCode);
            }

        }


    }
}