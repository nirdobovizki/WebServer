using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Tests
{

    [TestClass]
    public class FolderEndpoints
    {
        private HttpClient _client = new HttpClient();
        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
        }

        [TestMethod]
        public async Task GetFile()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeFolder("/folder",".");
            using(var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/folder/TextFile1.txt");
                Assert.AreEqual(System.Net.HttpStatusCode.OK, res.StatusCode);
                var content = await res.Content.ReadAsStringAsync();
                Assert.AreEqual("abc", content);
            }

        }

        [TestMethod]
        public async Task FileDoesNotExist()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeFolder("/folder", ".");
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/folder/nofile.txt");
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, res.StatusCode);
            }
        }


    /*    [TestMethod]
        public async Task TryingToGetAnotherFolder()
        {
            var builder = new WebServerBuilder();
            builder.HttpPort = TestSettings.HttpPort;
            builder.LocalOnly = true;
            builder.ExposeFolder("/folder", ".");
            using (var server = builder.BuildAndStart())
            {
                var res = await _client.GetAsync($"http://localhost:{TestSettings.HttpPort}/folder/../nofile.txt");
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, res.StatusCode);
            }
        }*/


    }
}