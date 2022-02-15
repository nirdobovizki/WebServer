using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{

    public abstract class LambdaEndpointHandler : TypedParameterHandler<(int[] paramMap, Delegate callback), Delegate>
    {
        private const int QUERY = -91;
        private const int BODY = -92;
        private List<(Type, Func<IHttpContext, object>)> _typedParamters = new List<(Type, Func<IHttpContext, object>)>();

        protected abstract Task TransformResultIntoStream(object result, IHttpContext context);
        protected abstract Task<object> TransformBodyToObject(Stream inputStream, Type expectedType);

        protected void AddTypedParameter(Type paramterType, Func<IHttpContext, object> getValue)
        {
            _typedParamters.Add((paramterType, getValue));
        }

        public LambdaEndpointHandler()
        {
            AddTypedParameter(typeof(IHttpContext), ctxt => ctxt);
            AddTypedParameter(typeof(IHttpRequest), ctxt => ctxt.Request);
            AddTypedParameter(typeof(IHttpResponse), ctxt => ctxt.Response);
            AddTypedParameter(typeof(Stream), ctxt => ctxt.Request.InputStream);
        }

        protected override (int[] paramMap, Delegate callback) PrepareHandlerData(Delegate callback, List<string> paramNames)
        {
            var callbackParams = callback.Method.GetParameters();
            var result = new int[callbackParams.Length];
            var usedPathParams = new bool[paramNames.Count];
            for (int i = 0; i < result.Length; i++)
            {
                var pName = callbackParams[i].Name;
                var pType = callbackParams[i].ParameterType;
                var inTypedList = _typedParamters.FindIndex(x=>x.Item1 == pType);
                if (inTypedList!=-1) result[i] = -inTypedList-1;
                else
                {
                    var nameIdx = paramNames.IndexOf(pName);
                    if (nameIdx != -1)
                    {
                        usedPathParams[nameIdx] = true;
                        result[i] = nameIdx;
                    }
//                  else if (callbackParams[i].GetCustomAttributes(typeof(BodyAttribute), true) != null)
                    else if (pName == "body")
                    {
                        result[i] = BODY;
                    }
                    else
                    {
                        result[i] = QUERY;
                    }
                }
            }
            if (!usedPathParams.All(i => i)) throw new ArgumentException($"path parameter {paramNames[usedPathParams.ToList().IndexOf(false)]} with no method argument");
            return (result,callback);
        }

        protected override async Task ProcessRequest(IHttpContext context, List<string> paramValues, (int[] paramMap, Delegate callback) data)
        {
            var callParamValues = new object[data.paramMap.Length];
            var callParamDefs = data.callback.Method.GetParameters();
            for (int i = 0; i < callParamValues.Length; i++)
            {
                if(data.paramMap[i] == QUERY)
                {
                    callParamValues[i] = RequestParamConverter.ConvertTo(callParamDefs[i].ParameterType, context.Request.QueryString[callParamDefs[i].Name]);
                }
                else if(data.paramMap[i] == BODY)
                {
                    callParamValues[i] = await TransformBodyToObject(context.Request.InputStream, callParamDefs[i].ParameterType);
                }
                else if(data.paramMap[i] < 0)
                {
                    callParamValues[i] = await ResolveTask(_typedParamters[-data.paramMap[i] - 1].Item2(context));
                }
                else
                { 
                    callParamValues[i] = RequestParamConverter.ConvertTo(callParamDefs[i].ParameterType, paramValues[data.paramMap[i]]);
                }
            }

            var result = data.callback.DynamicInvoke(callParamValues);

            if(data.callback.Method.ReturnType == typeof(void))
            {
                return;
            }

            if(result.GetType() == typeof(Task))
            {
                await (Task)result;
                return;
            }

            if (result is Task)
            {
                result = await ResolveTask(result);
            }

            bool statusCodeExplicit = false;
            if(result is WebResult)
            {
                var wr = (WebResult)result;
                result = wr.Data;
                context.Response.StatusCode = wr.Status;
                statusCodeExplicit = true;
            }

            if(result == null)
            {
                if (!statusCodeExplicit) throw new NotFoundException("");
                return;
            }

            await TransformResultIntoStream(result, context);
        }

        private async Task<object> ResolveTask(object obj)
        {
            if (obj is Task)
            {
                await(Task)obj;
                return obj.GetType().GetProperty("Result").GetValue(obj, null);
            }
            return obj;
        }
    }
}
