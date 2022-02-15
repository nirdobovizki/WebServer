using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Tests
{

    [TestClass]
    public class PostRequests
    {
        private HttpClient _client = new HttpClient();
        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
        }

        [TestMethod]
        public async Task PostPathParamters()
        {
            int gotValue = 0;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", (int id) =>
            { 
                gotValue = id;
                return "abc";
            });
            using(var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual(8, gotValue);
            }

        }

        [TestMethod]
        public async Task PostQueryParamters()
        {
            int gotValue = 0;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets", (int id) =>
            {
                gotValue = id;
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets?id=8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual(8, gotValue);
            }

        }

        [TestMethod]
        public async Task PostBodyAsStream()
        {
            string? gotValue = null;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", (int id,Stream data) =>
            {
                var buf = new byte[3];
                data.Read(buf, 0, buf.Length);
                gotValue = Encoding.UTF8.GetString(buf);
                data.Close();
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual("abc", gotValue);
            }

        }

        [TestMethod]
        public async Task PostAsyncBodyAsStream()
        {
            string? gotValue = null;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", async (int id, Stream data) =>
            {
                var buf = new byte[3];
                await data.ReadAsync(buf, 0, buf.Length);
                gotValue = Encoding.UTF8.GetString(buf);
                data.Close();
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual("abc", gotValue);
            }

        }



        [TestMethod]
        public async Task PostReturnValue()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", (int id) =>
            {
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("\"abc\"", contant.Result);
            }

        }

        [TestMethod]
        public async Task PostAsyncReturnValue()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", async (int id) =>
            {
                await Task.Yield();
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("\"abc\"", contant.Result);
            }

        }

        [TestMethod]
        public async Task PostReturnValueWithStatusCode()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", (int id) =>
            {
                return new WebResult<string>(System.Net.HttpStatusCode.Gone, "abc");
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.Gone, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("\"abc\"", contant.Result);
            }

        }

        [TestMethod]
        public async Task PostAsyncReturnValueWithStatusCode()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", async (int id) =>
            {
                await Task.Yield();
                return new WebResult<string>(System.Net.HttpStatusCode.Gone, "abc");
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.Gone, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("\"abc\"", contant.Result);
            }

        }

        [TestMethod]
        public async Task PostWithProgramError()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", string (int id) =>
            {
                throw new Exception();
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, res.StatusCode);
            }

        }

        [TestMethod]
        public async Task PostWithWebServerException()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/widgets/{id}", string (int id) =>
            {
                throw new NotFoundException("abc");
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("abc", contant.Result);
            }

        }

        [TestMethod]
        public async Task PostNotExists()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, res.StatusCode);
            }

        }


    }
}