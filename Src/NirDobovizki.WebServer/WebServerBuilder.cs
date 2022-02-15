using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NirDobovizki.WebServer.Advanced;

namespace NirDobovizki.WebServer
{
    public class WebServerBuilder : IAdvancedWebServerBuilder
    {
        private WebServerImpl.PathNode[] _methods = new WebServerImpl.PathNode[4]
        { new WebServerImpl.PathNode(), new WebServerImpl.PathNode(), new WebServerImpl.PathNode(), new WebServerImpl.PathNode()};

        private HashSet<IHandler> _initializedHandlers = new HashSet<IHandler>();

        public IAdvancedWebServerBuilder Advanced => this;
        public int HttpPort { get; set; }
        public int HttpsPort { get; set; }
        public bool LocalOnly { get; set; }
        public bool NoCache { get; set; } = true;
        public IHandler NotFoundHandler { get; set; } = new NotFoundHandler();
        public List<IRequestFilter> RequestFilters { get; } = new List<IRequestFilter>();

        Func<int, int, bool, IHttpEngine> IAdvancedWebServerBuilder.HttpEngineFactory { get; set; } = (http, https, local) => new HttpListenerEngine(http, https, local);

        public bool CORSAllowAll { get; set; }
        public List<string> CORSAllowedOrigins { get; } = new List<string>();

        public IJsonSerializer JsonSerializer { get; set; } = new DefaultJsonSerializer();
        public IMimeMapper MimeMapper { get; set; } = new DefaultMimeMapper();
        public IWebServer BuildAndStart()
        {
            if(NoCache)
            {
                RequestFilters.Add(new NoCacheFilter());
            }
            var server = new WebServerImpl(Advanced.HttpEngineFactory(HttpPort,HttpsPort,LocalOnly),
                _methods, NotFoundHandler, RequestFilters.ToArray(),
                CORSAllowAll, CORSAllowedOrigins.ToArray());
            _methods = new WebServerImpl.PathNode[4];
            return server;
        }

 
        void IAdvancedWebServerBuilder.AddPathHandler(WebServerHttpMethod method, string virtualPath, IHandler handler, object handlerParameter)
        {
            if (!virtualPath.StartsWith("/")) throw new ArgumentException("virtual path must start with /");
            if (virtualPath.EndsWith("/")) throw new ArgumentException("virtual path must not end with /");
            if (!_initializedHandlers.Contains(handler)) InitHandler(handler);
            var paramNames = new List<string>();
            var current = _methods[(int)method];
            var path = virtualPath.Split('/');
            for (int i = 0; i < path.Length; i++)
            {
                if(path[i] == "*")
                {
                    if(i == path.Length - 1)
                    {
                        current.isFolder = true;
                    }
                    else
                    {
                        throw new ArgumentException("* can only be used at end of path");
                    }
                }
                else if (path[i].StartsWith("{"))
                {
                    if (!path[i].EndsWith("}")) throw new ArgumentException("paramter path componenets must end with }");
                    if (path[i].Length == 2) throw new ArgumentException("paramter must have a name");
                    if (path[i].Contains(".")) throw new ArgumentException("paramter name may not contain dots");
                    if (current.ParamChild == null)
                    {
                        current.ParamChild = new WebServerImpl.PathNode();
                    }
                    paramNames.Add(path[i].Substring(1, path[i].Length - 2));
                    current = current.ParamChild;
                }
                else 
                if (current.Children.TryGetValue(path[i], out var next))
                {
                    current = next;
                }
                else
                {
                    if (path[i].Contains("{")) throw new ArgumentException("path can only contain { at begining of component");
                    if (path[i].Contains("}")) throw new ArgumentException("path can only contain } at end of component starting with {");
                    next = new WebServerImpl.PathNode();
                    current.Children.Add(path[i], next);
                    current = next;
                }
            }
            if (current.Handler != null) throw new ArgumentException("path conflicts with previous setting");
            current.Handler = handler;
            current.HandlerData = handler.PrepareHandlerData(handlerParameter, paramNames);
        }

        private void InitHandler(IHandler handler)
        {
            Dictionary<Type, Func<object>> supportedTypes = new Dictionary<Type, Func<object>>
            {
                {typeof(IJsonSerializer), ()=>JsonSerializer},
                {typeof(IMimeMapper), ()=>MimeMapper},
            };

            var initMethod = handler.GetType().GetMethod("Init");
            if (initMethod == null) return;
            var initParams = initMethod.GetParameters();
            var paramValues = new object[initParams.Length];
            for(int i = 0; i < initParams.Length; i++)
            {
                if(supportedTypes.TryGetValue(initParams[i].ParameterType,out var getter))
                {
                    paramValues[i] = getter();
                }
                else
                {
                    throw new Exception($"{handler.GetType().Name}.Init wants a paramter of type {initParams[i].ParameterType} that WebServerBuilder does not know how to provide");
                }
            }
            initMethod.Invoke(handler, paramValues);
            _initializedHandlers.Add(handler);
        }
    }
}
