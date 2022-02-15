using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Tests
{

    [TestClass]
    public class CORS
    {
        private HttpClient _client = new HttpClient();
        
        public CORS()
        {
            _client.DefaultRequestHeaders.Add("Origin", "http://example.com");
        }
        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
        }

        [TestMethod]
        public async Task CORSGetAllowAll()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/get",()=>"");
            builder.CORSAllowAll = true;
            using(var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/get");
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual(res.Headers.GetValues("Access-Control-Allow-Origin").Single(), "*");
            }
        }

        [TestMethod]
        public async Task CORSGetAllowed()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/get", () => "");
            builder.CORSAllowAll = false;
            builder.CORSAllowedOrigins.Add("http://example.com");
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/get");
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual(res.Headers.GetValues("Access-Control-Allow-Origin").Single(), "http://example.com");
            }
        }

        [TestMethod]
        public async Task CORSGetDenied()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/get", () => "");
            builder.CORSAllowAll = false;
            builder.CORSAllowedOrigins.Add("http://example.org");
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/get");
                Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, res.StatusCode);
            }
        }

        [TestMethod]
        public async Task CORSPrefilightAllowAll()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/post", () => "");
            builder.CORSAllowAll = true;
            using (var server = builder.BuildAndStart())
            {
                HttpRequestMessage r = new HttpRequestMessage();
                r.Method = HttpMethod.Options;
                r.RequestUri = new Uri($"http://localhost:{TestSettings.HttpPort}/post");
                var res = await _client.SendAsync(r);
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual(res.Headers.GetValues("Access-Control-Allow-Origin").Single(), "*");
            }
        }

        [TestMethod]
        public async Task CORSPreflightAllowed()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/post", () => "");
            builder.CORSAllowAll = false;
            builder.CORSAllowedOrigins.Add("http://example.com");
            using (var server = builder.BuildAndStart())
            {
                HttpRequestMessage r = new HttpRequestMessage();
                r.Method = HttpMethod.Options;
                r.RequestUri = new Uri($"http://localhost:{TestSettings.HttpPort}/post");
                var res = await _client.SendAsync(r);
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                Assert.AreEqual(res.Headers.GetValues("Access-Control-Allow-Origin").Single(), "http://example.com");
            }
        }

        [TestMethod]
        public async Task CORSPreflightDenied()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/post", () => "");
            builder.CORSAllowAll = false;
            builder.CORSAllowedOrigins.Add("http://example.org");
            using (var server = builder.BuildAndStart())
            {
                HttpRequestMessage r = new HttpRequestMessage();
                r.Method = HttpMethod.Options;
                r.RequestUri = new Uri($"http://localhost:{TestSettings.HttpPort}/post");
                var res = await _client.SendAsync(r);
                Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, res.StatusCode);
            }
        }


    }
}