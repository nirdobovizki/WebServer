using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public class JsonApiHandler : LambdaEndpointHandler
    {
        private IJsonSerializer _serializer;

        public void Init(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override async Task<object> TransformBodyToObject(Stream inputStream, Type expectedType)
        {
            using (var reader = new StreamReader(inputStream))
            {
                var content = await reader.ReadToEndAsync();
                try
                {
                    return _serializer.Deserialize(content, expectedType);
                }
                catch(Exception ex)
                {
                    throw new BadRequestException("Can't parse json body: " + ex.Message);
                }
            }
        }

        protected override async Task TransformResultIntoStream(object result, IHttpContext context)
        {
            var buffer = Encoding.UTF8.GetBytes(_serializer.Serialize(result));
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentLength64 = buffer.Length;
            context.Response.Headers.Add("Content-Type", "application/json");
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}
