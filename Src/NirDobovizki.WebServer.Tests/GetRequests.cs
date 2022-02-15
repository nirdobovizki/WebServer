using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Tests
{

    [TestClass]
    public class GetRequests
    {
        private HttpClient _client = new HttpClient();
        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
        }

        [TestMethod]
        public async Task GetPathParamters()
        {
            int gotValue = 0;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets/{id}", (int id) =>
            { 
                gotValue = id;
                return "abc";
            });
            using(var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8");
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual(8, gotValue);
            }

        }

        [TestMethod]
        public async Task GetQueryParamters()
        {
            int gotValue = 0;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets", (int id) =>
            {
                gotValue = id;
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets?id=8");
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual(8, gotValue);
            }

        }

        public async Task GetOptionalQueryParamters()
        {
            int gotValue = 0;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets", (int? id) =>
            {
                gotValue = id ?? 0;
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets?id=8");
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual(8, gotValue);
            }

        }

        [TestMethod]
        public async Task GetMissingQueryParamters()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets", (int id) =>
            {
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets");
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, res.StatusCode);
            }

        }

        [TestMethod]
        public async Task GetMissingOptionalQueryParamters()
        {
            bool gotValue = false;
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets", (int? id) =>
            {
                if (id == null) gotValue = true;
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets");
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.IsTrue(gotValue);
            }

        }


        [TestMethod]
        public async Task GetReturnValue()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets/{id}", (int id) =>
            {
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8");
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("\"abc\"", contant.Result);
            }

        }

        [TestMethod]
        public async Task GetAsyncReturnValue()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets/{id}", async (int id) =>
            {
                await Task.Yield();
                return "abc";
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8");
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("\"abc\"", contant.Result);
            }

        }

        [TestMethod]
        public async Task GetReturnValueWithStatusCode()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets/{id}", (int id) =>
            {
                return new WebResult<string>(System.Net.HttpStatusCode.Gone, "abc");
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8");
                Assert.AreEqual(System.Net.HttpStatusCode.Gone, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("\"abc\"", contant.Result);
            }

        }

        [TestMethod]
        public async Task GetAsyncReturnValueWithStatusCode()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets/{id}", async (int id) =>
            {
                await Task.Yield();
                return new WebResult<string>(System.Net.HttpStatusCode.Gone, "abc");
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8");
                Assert.AreEqual(System.Net.HttpStatusCode.Gone, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("\"abc\"", contant.Result);
            }

        }

        [TestMethod]
        public async Task GetWithProgramError()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets/{id}", string (int id) =>
            {
                throw new Exception();
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8");
                Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, res.StatusCode);
            }

        }

        [TestMethod]
        public async Task GetWithWebServerException()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/widgets/{id}", string (int id) =>
            {
                throw new NotFoundException("abc");
            });
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8");
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, res.StatusCode);
                var contant = res.Content.ReadAsStringAsync();
                Assert.AreEqual("abc", contant.Result);
            }

        }

        [TestMethod]
        public async Task GetNotExists()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8");
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, res.StatusCode);
            }

        }



    }
}