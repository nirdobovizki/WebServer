using System;
using System.Collections.Generic;
using System.Text;

namespace NirDobovizki.WebServer.Advanced
{
    public class DefaultMimeMapper : IMimeMapper
    {
        public string DefaultMimeType { get; set; } = null;
        public string GetMimeType(string filename)
        {
            if(filename == null) return DefaultMimeType;
            var dotLocation = filename.LastIndexOf('.');
            if (dotLocation <= 0 || dotLocation == filename.Length-1) return DefaultMimeType;
            var extention = filename.Substring(dotLocation + 1);
            if(Generated.MimeTypes.MimeTypeMap.TryGetValue(extention, out var mimeType))
                return mimeType;
            return DefaultMimeType;
            
        }
    }
}
