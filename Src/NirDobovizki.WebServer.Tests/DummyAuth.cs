using Microsoft.VisualStudio.TestTools.UnitTesting;
using NirDobovizki.WebServer.Advanced;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Tests
{
    [TestClass]
    public class DummyAuth
    {
        class AuthFilter : IRequestFilter
        {
            public string? TryLogin(string name, string password)
            {
                if (name == "dummy" && password == "dummy") return "12";
                return null;
            }

            public Task AfterExecuteHandlerSuccessfully(IHttpContext context)
            {
                return Task.CompletedTask;
            }

            public Task<bool> BeforeExecuteHandler(IHttpContext context)
            {
                if (context.Request.UrlPath == "/login") return Task.FromResult(true);
                var token = context.Request.Headers.GetValues("Authorization")?.SingleOrDefault();
                if (token != null && token.StartsWith("Bearer "))
                {
                    token = token.Substring(7).Trim();
                    if (token == "12")
                    {
                        return Task.FromResult(true);
                    }
                }
                context.Response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return Task.FromResult(false);
            }

            public Task<bool> BeforeParseRequest(IHttpContext context)
            {
                return Task.FromResult(true); ;
            }

            public void ExceptionDuringProcessing(IHttpContext context, Exception exception)
            {
            }
        }

        public class LoginParamRequest
        {
            public string? name;
            public string? password;
        }

        public class LoginParamReply
        {
            public string? token;
        }

        private AuthFilter? _auth;
        private IWebServer? _webServer;
        private bool _callHappened;

        [TestInitialize]
        public void Init()
        {
            _auth = new AuthFilter();
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.RequestFilters.Add(_auth);
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/login", (LoginParamRequest body) =>
            {
                if (body == null || body.name == null || body.password == null)
                    throw new BadRequestException("missing data");
                var token = _auth.TryLogin(body.name, body.password);
                if (token == null)
                    throw new NotFoundException("bad name of password");
                return new LoginParamReply { token = token };
            });
            builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/do", () => { _callHappened = true; return ""; });
            _webServer = builder.BuildAndStart();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _webServer?.Dispose();
        }

        [TestMethod]
        public async Task CallWithNoLogin()
        {
            _callHappened = false;
            using(var client = new HttpClient())
            {
                var res = await client.GetAsync($"http://localhost:{TestSettings.HttpPort}/do");
                Assert.AreEqual(HttpStatusCode.Forbidden, res.StatusCode);
                Assert.IsFalse(_callHappened);
            }
        }

        [TestMethod]
        public async Task CallWithIncompleteLoginData()
        {
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync($"http://localhost:{TestSettings.HttpPort}/login", new StringContent("{'name':'dummy'}"));
                Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);
            }
        }

        [TestMethod]
        public async Task CallWithIncorrectLoginData()
        {
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync($"http://localhost:{TestSettings.HttpPort}/login", new StringContent("{'name':'dummy','password':'smart'}"));
                Assert.AreEqual(HttpStatusCode.NotFound, res.StatusCode);
            }
        }

        [TestMethod]
        public async Task CompleteFlow()
        {
            _callHappened = false;
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync($"http://localhost:{TestSettings.HttpPort}/login", new StringContent("{'name':'dummy','password':'dummy'}"));
                Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
                var loginReplyStr = await res.Content.ReadAsStringAsync();
                var loginReply = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginParamReply>(loginReplyStr);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginReply?.token);
                res = await client.GetAsync($"http://localhost:{TestSettings.HttpPort}/do");
                Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
                Assert.IsTrue(_callHappened);
            }
        }

        [TestMethod]
        public async Task FakeToken()
        {
            _callHappened = false;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "7");
                var res = await client.GetAsync($"http://localhost:{TestSettings.HttpPort}/do");
                Assert.AreEqual(HttpStatusCode.Forbidden, res.StatusCode);
                Assert.IsFalse(_callHappened);
            }
        }





    }
}