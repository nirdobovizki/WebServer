# C# Embedded Web Server

A simple, light-weight web server designed to be embedded in
a local .net application.

Desiged for:
* UI for smart devices, IoT devices or appliences coded in .net
* UI for Windows services 
* API for desktop applications

Support:
* Network and localhost-only modes
* HTTP and HTTPS
* WebSockets
* Easy CORS configuration
* and much more

This server is optimized for providing UI or APIs for local applications
and devices, it is not intended to be used in internet servers

Example - create a localhost-only POST endpoint that 
recieves JSON and replies with JSON, with full CORS support

    var builder = new WebServerBuilder();
    builder.HttpPort = 8888;
    builder.LocalOnly = true;
    builder.CORSAllowedOrigins.Add("http://localhost:8888");
    builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, 
          "/sendMessage", (MessageDataModel body) =>
    {
        // do stuff
        return new { property='value' };
    }
    var server = builder.CreateAndStart();


Example - create a network-exposed web sockets endpoint and
send updates to all connected clients

    var hub = new JsonHub();
    var builder = new WebServerBuilder();
    builder.HttpPort = 8888;
    builder.CORSAllowAll = true;
    builder.ExposePubSubSubscribeEndpoint("/ws", hub);
    var server = builder.CreateAndStart();

    // later

    hub.Publish(new { message="Hi all"});

How to use
---

* Create a WebServerBuilder
* Set HttpPort, HttpsPort or both (see below about how to enable HTTPS)
* If the server is accessed by a web browser set CORSAllowAll or CORSAllowedOrigins
* Add the endpoints exposed by the server
     * ExposeFile - will map a URL to a single file
     * ExposeFolder - will map a URL to a folder
     * ExposeJsonEndpoint - will map a URL to a lambda that supports JSON
     * ExposeWebSocketEndpoint - will map a URL to a lambda that supports web sockets
     * ExposePubSubSubscribeEndpoint - will map a URL to a Hub you can use to easily send data to multiple clients

Paramters:
---

The server supports paramters that are part of the path:

    builder.ExposeJsonEndpoint(WebServerHttpMethod.GET, 
          "/widget/{id}", (int id) =>

The paramter is defined in the path string using curly braces
the value will be passed to the paramter with the same name in
the supplied lambda function

JSON payload in the POST request body:

    builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, 
          "/sendMessage", (MessageDataModel body)

The payload will be parsed using Newtonsoft Json.net and passed
to the lambda in the parameter named *body*

And query parameters:

and parameter of the lambda that is not part of the path and isn't
called body will be filled with data from the query string




