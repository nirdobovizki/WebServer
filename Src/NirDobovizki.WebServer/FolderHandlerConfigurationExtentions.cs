using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer
{
    public static class FolderHandlerConfigurationExtentions
    {
        private static readonly Advanced.FolderHandler _handler = new Advanced.FolderHandler();
        public static void ExposeFolder(this WebServerBuilder builder, string virtualPath, string fileSystemPath)
        {
            if (virtualPath == "/") virtualPath = "/*";
            else if (!virtualPath.EndsWith("/*")) virtualPath += "/*";
            builder.Advanced.AddPathHandler(WebServerHttpMethod.GET, virtualPath, _handler, fileSystemPath);
        }
    }
}
