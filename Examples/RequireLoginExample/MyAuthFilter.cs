using NirDobovizki.WebServer.Advanced;

internal class MyAuthFilter : IRequestFilter
{
    private MyAuthServer _authServer;

    public MyAuthFilter(MyAuthServer authServer)
    {
        _authServer = authServer;
    }

    public Task AfterExecuteHandlerSuccessfully(IHttpContext context)
    {
        return Task.CompletedTask;
    }

    public Task<bool> BeforeExecuteHandler(IHttpContext context)
    {
        // allow requests required for login
        if (context.Request.UrlPath == "/login" ||
            context.Request.UrlPath == "/" ||
            context.Request.UrlPath == "/index.htm" ||
            context.Request.UrlPath.StartsWith("/assets/"))
        {
            return Task.FromResult(true);
        }

        var token = context.Request.Headers.GetValues("Authorization")?.SingleOrDefault();
        if (token != null && token.StartsWith("Bearer "))
        {
            token = token.Substring(7).Trim();
            if (_authServer.IsTokenValid(token))
            {
                return Task.FromResult(true);
            }
        }
        context.Response.StatusCode = System.Net.HttpStatusCode.Forbidden;
        return Task.FromResult(false);

    }

    public Task<bool> BeforeParseRequest(IHttpContext context)
    {
        return Task.FromResult(true);
    }

    public void ExceptionDuringProcessing(IHttpContext context, Exception exception)
    {
    }
}