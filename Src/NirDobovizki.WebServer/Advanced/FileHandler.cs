using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public class FileHandler : TypedParameterHandler<string, string>
    {
        private IMimeMapper _mimeMapper;

        public void Init(IMimeMapper mimeMapper)
        {
            _mimeMapper = mimeMapper;
        }

        protected override string PrepareHandlerData(string handlerParams, List<string> paramNames)
        {
            return handlerParams;
        }

        protected override async Task ProcessRequest(IHttpContext context, List<string> paramValues, string filesystemPath)
        {
            try
            {
                byte[] buffer = new byte[1024];
                using (var stream = new FileStream(filesystemPath, FileMode.Open))
                {
                    using (var writer = context.Response.OutputStream)
                    {
                        var mimeType = _mimeMapper.GetMimeType(Path.GetFileName(filesystemPath));
                        if (mimeType != null)
                            context.Response.Headers.Add("Content-Type", mimeType);
                        while (true)
                        {
                            var count = await stream.ReadAsync(buffer, 0, buffer.Length);
                            if (count == 0) return;
                            await writer.WriteAsync(buffer, 0, count);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new NotFoundException("File does not exist");
            }
        }
    }
}
