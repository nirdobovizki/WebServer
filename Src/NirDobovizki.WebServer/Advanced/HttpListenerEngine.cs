using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public class HttpListenerEngine : IHttpEngine
    {
        internal class ContextImpl : IHttpContext, IHttpRequest, IHttpResponse
        {
            private HttpListenerContext _innerContext;

            public ContextImpl(HttpListenerContext innerContext)
            {
                _innerContext = innerContext;
            }

            public IHttpRequest Request => this;

            public IHttpResponse Response => this;

            public string HttpMethod => _innerContext.Request.HttpMethod;

            public Uri Url => _innerContext.Request.Url;

            private string _urlPath;
            public string UrlPath
            {
                get
                {
                    if(_urlPath == null)
                    {
                        var pathAndParams = _innerContext.Request.RawUrl;
                        var sep = pathAndParams.IndexOf('?');
                        _urlPath = sep == -1 ? pathAndParams : pathAndParams.Substring(0, sep);
                    }
                    return _urlPath;
                }
                set 
                {
                    _urlPath = value;
                }
            }

            public HttpStatusCode StatusCode { get => (HttpStatusCode)_innerContext.Response.StatusCode; set => _innerContext.Response.StatusCode=(int)value; }

            public Stream OutputStream => _innerContext.Response.OutputStream;

            public Stream InputStream => _innerContext.Request.InputStream;

            public NameValueCollection QueryString => _innerContext.Request.QueryString;


            long IHttpResponse.ContentLength64 { get => _innerContext.Response.ContentLength64; set => _innerContext.Response.ContentLength64 = value; }
            Encoding IHttpResponse.ContentEncoding { get => _innerContext.Response.ContentEncoding; set => _innerContext.Response.ContentEncoding = value; }
            public string InFolderPath { get; set; }

            NameValueCollection IHttpRequest.Headers => _innerContext.Request.Headers;

            NameValueCollection IHttpResponse.Headers => _innerContext.Response.Headers;

            public async Task<WebSocket> UpgradeToWebSocket()
            {
                var wsc = await _innerContext.AcceptWebSocketAsync(null);
                return wsc.WebSocket;
            }

            public void End()
            {
                _innerContext.Response.Close();
            }
        }


        private HttpListener _listner;

        public HttpListenerEngine(int httpPort, int httpsPort, bool localhostOnly)
        {
            _listner = new HttpListener();
            if (httpPort != 0) _listner.Prefixes.Add($"http://{(localhostOnly ? "localhost" : "*")}:{httpPort}/");
            if (httpsPort != 0) _listner.Prefixes.Add($"https://{(localhostOnly ? "localhost" : "*")}:{httpPort}/");
            _listner.Start();
        }

        public async Task<IHttpContext> AwaitNextConnection()
        {
            try
            { 
                return new ContextImpl(await _listner.GetContextAsync());
            }
            catch (HttpListenerException)
            {
                throw new HttpEngineDisposedException();
            }
        }

        public void Dispose()
        {
            _listner?.Stop();
            _listner = null;
        }
    }

 
}
