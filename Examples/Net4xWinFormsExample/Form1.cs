using NirDobovizki.WebServer;
using NirDobovizki.WebServer.PubSub;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Net4xWinFormsExample
{
    public partial class Form1 : Form
    {
        private IWebServer _webServer;
        private JsonHub _hub;

        public Form1()
        {
            InitializeComponent();

            _hub = new JsonHub();
            var builder = new WebServerBuilder();
            builder.HttpPort = 8888;
            builder.LocalOnly = true;
            builder.CORSAllowedOrigins.Add("http://localhost:8888");
            builder.ExposeFolder("/", "WebUI");
            builder.ExposeJsonEndpoint(WebServerHttpMethod.POST, "/sendMessage", (MessageDataModel body) =>
              {
                  _hub.Publish(body);
                  RecText.Text = body.Message + Environment.NewLine + Environment.NewLine + RecText.Text;
                  return new { };
              });
            builder.ExposePubSubSubscribeEndpoint("/listen", _hub);
            _webServer = builder.BuildAndStart();
        }

        private void SendBtn_Click(object sender, EventArgs e)
        {
            _hub.Publish(new MessageDataModel { Message = SendText.Text });
            RecText.Text = SendText.Text + Environment.NewLine + Environment.NewLine + RecText.Text;
        }

        private void OpenWebBtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "http://localhost:8888",
            });
        }
    }
}
