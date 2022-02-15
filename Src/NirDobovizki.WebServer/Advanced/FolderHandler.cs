using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public class FolderHandler : TypedParameterHandler<string, string>
    {
        private IMimeMapper _mimeMapper;

        public void Init(IMimeMapper mimeMapper)
        {
            _mimeMapper = mimeMapper;
        }

        protected override string PrepareHandlerData(string handlerParams, List<string> paramNames)
        {
            return Path.GetFullPath(handlerParams);
        }

        protected override async Task ProcessRequest(IHttpContext context, List<string> paramValues, string filesystemPath)
        {
            if (context.Request.InFolderPath?.Contains("..") ?? false) throw new BadRequestException("Bad path");
            string filePath;
            if ((context.Request.InFolderPath?.Length??0)==0 )
            {
                filePath = Path.GetFullPath(Path.Combine(filesystemPath, "index.html"));

            }
            else
            {
                filePath = Path.GetFullPath(Path.Combine(filesystemPath, context.Request.InFolderPath));
            }
            if (!filePath.StartsWith(filesystemPath)) throw new BadRequestException("Bad path");

            try
            {
                byte[] buffer = new byte[1024];
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    using (var writer = context.Response.OutputStream)
                    {
                        var mimeType = _mimeMapper.GetMimeType(Path.GetFileName(filePath));
                        if(mimeType != null)
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
