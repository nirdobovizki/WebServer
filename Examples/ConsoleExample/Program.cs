
using NirDobovizki.WebServer;
using NirDobovizki.WebServer.PubSub;

// the hub is used to send notifications to clients
var hub = new JsonHub();

// setup local server
var builder = new WebServerBuilder();
builder.HttpPort = 8888;
builder.LocalOnly = true;
builder.CORSAllowedOrigins.Add("http://localhost:8888");

// expose html folder
builder.ExposeFolder("/", "WebUI");

// an endpoint for the clients t send messages
builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/sendMessage", (MessageDataModel body) =>
{
    hub.Publish(body);
    Console.WriteLine(body.Message);
    return new { };
});

// and a websocket endpoint for the clients to get updates
builder.ExposePubSubSubscribeEndpoint("/listen", hub);

// start server
var webServer = builder.BuildAndStart();


Console.WriteLine("Open web browser on http://localhost:8888");
Console.WriteLine("Type text and press enter to send");
while(true)
{
    var text = Console.ReadLine();
    if (text == null) break;
    hub.Publish(new MessageDataModel { Message = text });
}
