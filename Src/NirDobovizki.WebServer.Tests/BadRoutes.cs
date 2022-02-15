using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace NirDobovizki.WebServer.Tests
{

    [TestClass]
    public class BadRoutes
    {
        [TestMethod]
        public void InvalidRoutes()
        {
            var routes = new (string path, string error)[]
            {
                ("widgets","virtual path must start with /"),
                ("/widgets/","virtual path must not end with /"),
                ("/widgets/{","paramter path componenets must end with }"),
                ("/widgets/{}","paramter must have a name"),
                ("/widgets/{x","paramter path componenets must end with }"),
                ("/widgets/x{","path can only contain { at begining of component"),
                ("/widgets/{/name","paramter path componenets must end with }"),
                ("/widgets/}","path can only contain } at end of component starting with {"),
                ("/widgets/x}","path can only contain } at end of component starting with {"),
                ("/widgets/}x","path can only contain } at end of component starting with {"),
                ("/widgets/}/name","path can only contain } at end of component starting with {"),
                ("/widgets/*/name","* can only be used at end of path"),
                ("/widgets/{.}/name","paramter name may not contain dots"),
                ("/widgets/{x.x}/name","paramter name may not contain dots"),
                ("/widgets/{.x}/name","paramter name may not contain dots"),
                ("/widgets/{x.}/name","paramter name may not contain dots"),
                ("/widgets/{..}/name","paramter name may not contain dots"),
            };

            foreach(var route in routes)
            {
                var builder = new WebServerBuilder();
                try
                {
                    builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, route.path, () => "");
                    Assert.Fail($"route \"{route.path}\" should have failed but didn't (expected message:{route.error})");
                }
                catch(ArgumentException ex)
                {
                    Assert.AreEqual(route.error, ex.Message);
                }
            }
        }

        [TestMethod]
        public void MissingParameters()
        {
            var builder = new WebServerBuilder();
            try
            {
                builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/wisgets/{id}", () => "");
                Assert.Fail($"route with parameters not in method should have failed but didn't");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "path parameter id with no method argument");
            }

        }

        /*
        [TestMethod]
        public void GetWithBody()
        {
            var builder = new WebServerBuilder();
            try
            {
                builder.ExposeJsonEndpoint(HttpMethod.GET, "/wisgets/{id}", (int id, Stream body) => "");
                Assert.Fail($"get route with body should have failed but didn't");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "can't use body in get endpoints");
            }

        }
        */

    }
}