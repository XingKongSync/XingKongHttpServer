using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XingKongHttpServer;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8080;
            HttpServer httpServer = new HttpServer(port);
            httpServer.LoadController(typeof(Program), "Demo.Controller");
            httpServer.Start();


            Console.ReadLine();
            httpServer.Stop();
        }
    }
}
