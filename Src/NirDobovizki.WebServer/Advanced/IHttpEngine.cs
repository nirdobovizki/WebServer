using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public interface IHttpEngine : IDisposable
    {
        Task<IHttpContext> AwaitNextConnection();
    }
}
