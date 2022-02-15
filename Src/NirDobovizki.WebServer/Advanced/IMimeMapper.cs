using System;
using System.Collections.Generic;
using System.Text;

namespace NirDobovizki.WebServer.Advanced
{
    public interface IMimeMapper
    {
        string GetMimeType(string filename);
    }
}
