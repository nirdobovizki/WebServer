using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer
{
    public static class FileHandlerConfigurationExtentions
    {
        private static readonly Advanced.FileHandler _handler = new Advanced.FileHandler();
        public static void ExposeFile(this WebServerBuilder builder, string virtualPath, string fileSystemPath)
        {
            builder.Advanced.AddPathHandler(WebServerHttpMethod.GET, virtualPath, _handler, fileSystemPath);
        }
    }
}
