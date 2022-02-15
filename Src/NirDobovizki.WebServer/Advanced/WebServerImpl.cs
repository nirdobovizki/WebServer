using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace NirDobovizki.WebServer.Advanced
{
    public class WebServerImpl : IWebServer
    {
        
        internal class PathNode
        {
            public bool isFolder;
            public Dictionary<string, PathNode> Children = new Dictionary<string, PathNode>();
            public PathNode ParamChild;
            public Advanced.IHandler Handler;
            public object HandlerData;
        }

        private IHttpEngine _httpEngine;
        private PathNode[] _methods;
        private IHandler _notFound;
        private IRequestFilter[] _requestFilters;
        private bool _CORSAllowAll;
        private string[] _CORSAllowedOrigins;


        internal WebServerImpl(IHttpEngine httpEngine,
            PathNode[] methods, IHandler notFound,
            IRequestFilter[] requestFilters,
            bool CORSAllowAll,string[] CORSAllowedOrigins)
        {
            _httpEngine = httpEngine;
            _methods = methods;
            _notFound = notFound;
            _requestFilters = requestFilters;
            _CORSAllowAll = CORSAllowAll;
            _CORSAllowedOrigins = CORSAllowedOrigins;
            Start();
        }


        private (IHandler handler, object handlerData,
            List<string> pathParameters,bool found, 
            string inFolderPath) 
            ResolveHandlerForPath(
            WebServerHttpMethod method, 
            string virtualPath)
        {
            var inPathParams = new List<string>();
            var current = _methods[(int)method];
            var path = virtualPath.Split('/');
            for(int i=0;i<path.Length;i++)
            {
                if(current.Children.TryGetValue(path[i],out var next))
                {
                    current = next;
                }
                else if(current.ParamChild != null)
                {
                    inPathParams.Add(path[i]);
                    current = current.ParamChild;
                }
                else if(current.isFolder)
                {
                    return (current.Handler, current.HandlerData, inPathParams, true, string.Join("/", path.Skip(i)));
                }
                else
                {
                    return (_notFound, null, inPathParams, false, null);
                }
            }
            if(current.Handler == null)
            {
                return (_notFound, null, inPathParams, false, null);
            }
            return (current.Handler, current.HandlerData, inPathParams, true, null);
        }

 
 

        private async void Start()
        {
            await Task.Yield();
            while (true)
            {
                IHttpContext context;
                try
                {
                    context = await _httpEngine.AwaitNextConnection();
                }
                catch(HttpEngineDisposedException)
                {
                    return;
                }
                try
                {
                    await ProcessRequest(context);
                }
                catch (TargetInvocationException ex) when (ex.InnerException is WebServerException)
                {
                    try
                    {
                        context.Response.StatusCode = ((WebServerException)ex.InnerException).StatusCode;
                        using (var writer = new StreamWriter(context.Response.OutputStream))
                        {
                            writer.Write(ex.InnerException.Message);
                        }
                    }
                    catch
                    {
                        //
                    }
                }
                catch (WebServerException ex)
                {
                    try
                    {
                        context.Response.StatusCode = ex.StatusCode;
                        using (var writer = new StreamWriter(context.Response.OutputStream))
                        {
                            writer.Write(ex.Message);
                        }
                    }
                    catch
                    {
                        //
                    }
                }
                catch
                {
                    context.Response.StatusCode = HttpStatusCode.InternalServerError;
                    context.Response.End();
                }
            }
        }

        private async Task ProcessRequest(IHttpContext context)
        {
            try
            {
                if(!await BeforeParseRequest(context))
                {
                    context.Response.End();
                    return;
                }

                var request = context.Request;
                var options = request.HttpMethod == "OPTIONS";
                var origin = request.Headers["Origin"];

                if(origin != null)
                {
                    if(_CORSAllowAll)
                    {
                        context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    }
                    else if(_CORSAllowedOrigins.Any(o=>string.Compare(o,origin,StringComparison.OrdinalIgnoreCase)==0))
                    {
                        context.Response.Headers["Access-Control-Allow-Origin"] = origin;
                    }
                    else
                    {
                        // not allowed
                        context.Response.StatusCode=HttpStatusCode.Forbidden;
                        context.Response.End();
                        return;
                    }
                    
                    if(options)
                    {
                        // preflight
                        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE";
                        context.Response.End();
                        return;
                    }
                }

                var method =
                    request.HttpMethod == "GET" ? WebServerHttpMethod.GET :
                    request.HttpMethod == "POST" ? WebServerHttpMethod.POST :
                    request.HttpMethod == "PUT" ? WebServerHttpMethod.PUT :
                    request.HttpMethod == "DELETE" ? WebServerHttpMethod.DELETE :
                    throw new BadRequestException($"unsupported http method {context.Request.HttpMethod}");
                var handlerPack = ResolveHandlerForPath(method, request.UrlPath);
                request.InFolderPath = handlerPack.inFolderPath;


                if(!await BeforeExecuteHandler(context))
                {
                    context.Response.End();
                    return;
                }

                await handlerPack.handler.ProcessRequest(context, handlerPack.pathParameters, handlerPack.handlerData);

                await AfterExecuteHandlerSuccessfully(context);

            }
            catch (Exception ex)
            {
                ExceptionDuringProcessing(context, ex);
                throw;
            }
        }

        private void ExceptionDuringProcessing(IHttpContext context, Exception exception)
        {
            foreach(var current in _requestFilters)
            {
                current.ExceptionDuringProcessing(context, exception);
            }
        }

        private async Task AfterExecuteHandlerSuccessfully(IHttpContext context)
        {
            foreach (var current in _requestFilters)
            {
                await current.AfterExecuteHandlerSuccessfully(context);
            }
        }

        private async Task<bool> BeforeExecuteHandler(IHttpContext context)
        {
            foreach(var current in _requestFilters)
            {
                if(!await current.BeforeExecuteHandler(context))
                    return false;
            }
            return true;
        }

        private async Task<bool> BeforeParseRequest(IHttpContext context)
        {
            foreach (var current in _requestFilters)
            {
                if (!await current.BeforeParseRequest(context))
                    return false;
            }
            return true;
        }

        public void Dispose()
        {
            _httpEngine.Dispose();
        }
    }
}
