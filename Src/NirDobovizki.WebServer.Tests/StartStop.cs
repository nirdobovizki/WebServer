using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Tests
{
    [TestClass]
    public class StartStop
    {
        private HttpClient _client = new HttpClient();
        
        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
        }

        [TestMethod]
        public async Task DoTest()
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
            }
            try
            {
                await _client.PostAsync($"http://localhost:{TestSettings.HttpPort}/widgets/8", new StringContent("abc"));
                Assert.Fail("Server should have stopped");
            }
            catch(HttpRequestException)
            {
                //good
            }
        }



    }
}