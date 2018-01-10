using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XingKongHttpServer;

namespace Demo.Controller
{
    public class Hello : HttpRequestHandlerBase
    {
        public Hello()
        {
            Path = "/hello";
        }

        public override void Handler(HttpListenerRequest request, HttpListenerResponse response)
        {
            Text(response, "Hello World!");
        }
    }

    public class Home : HttpRequestHandlerBase
    {
        public Home()
        {
            Path = "/";
        }

        public override void Handler(HttpListenerRequest request, HttpListenerResponse response)
        {
            string html = "<html><head><title>Welcome</title></head><body>Welcome to XingKongHttpServer</body></html>";
            Html(response, html);
        }
    }
}
