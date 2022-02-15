using NirDobovizki.WebServer;

// my fake authentication system
var authServer = new MyAuthServer();

// start local web server
var builder = new WebServerBuilder();
builder.HttpPort = 8888;
builder.LocalOnly = true;
builder.CORSAllowedOrigins.Add("http://localhost:8888");

// add the filter that checks authentication 
builder.RequestFilters.Add(new MyAuthFilter(authServer));

// expose html
builder.ExposeFolder("/", "WebUI");

// login and logout calls
builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/login", (LoginRequest body) =>
{
    if (body == null || body.name == null || body.password == null)
        throw new BadRequestException("missing data");
    var token = authServer.TryLogin(body.name, body.password);
    if (token == null)
        throw new NotFoundException("bad name or password");
    return new LoginReply { token = token };
});
builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/logout", (NirDobovizki.WebServer.Advanced.IHttpRequest request) =>
{
var token = request.Headers.GetValues("Authorization")?.SingleOrDefault();
    if (token != null && token.StartsWith("Bearer "))
    {
        token = token.Substring(7).Trim();
        authServer.Logout(token);
    }
    return new { };
});

// fake "do something" so we have something to call
builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, "/doSomething", () => { return new { result = 12 }; });

// and start server
var webServer = builder.BuildAndStart();

Console.WriteLine("Open web browser on http://localhost:8888");
Console.WriteLine("Press any key to exit");
Console.ReadKey();



