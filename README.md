# XingKongHttpServer
Simple Http Server by C#
## 简介/Introduction
这个解决方案包含了一个简易的Http服务器的实现。

This is a implement of a simple Http Server.

注意：运行本程序需要管理员权限

Attention: Administrator privilege is required.

## 示例/Usage
### Step 1.

首先需要创建一个请求处理器类，并继承于HttpRequestHandlerBase，其中的Path为请求路径，Handler方法用于处理Http请求。

Firstly you need to create a request handler which is the subclass of HttpRequestHandlerBase.

```C#
namespace Demo.Controller
{
    public class Home : HttpRequestHandlerBase
    {
        public Home()
        {
            Path = "/";
        }

        public override void Handler(HttpListenerRequest request, HttpListenerResponse response)
        {
            string html = "<html><head><title>Welcome</title></head><body>Welcome to XingKongHttpServer</body></html>";
            //Content-Type: text/html
            Html(response, html);
        }
    }
}
```

### Step 2.

然后在你的主程序中创建该对象的实例，添加进HttpServer的路由中。

Create a instance of the class then add to the routher of the HttpServer.

```C#
namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8080;
            HttpServer httpServer = new HttpServer(port);
            httpServer.AddPath(new Home());
            httpServer.Start();

            Console.ReadLine();
            httpServer.Stop();
        }
    }
}
```

### Step 3.

至此，一个最简单的Http服务器已经搭建完成，编译并运行该程序，在浏览器中访问 http://localhost:8080 即可看到效果

注意：需要管理员权限运行。

OK, everthing is done, you can run the program and type "http://localhost:8080" on your browser, see what will happen.

Attention: Administrator privilege is required.

### Other

另外，推荐将所有的请求处理器放入某个命名空间下，例如：“Demo.Controller”，然后在主程序中采用如下写法，即可自动加载该命名空间下的全部请求处理器。

By the way, I recommand to put all of your request handlers in a namespace such as "Demo.Controller", then you can write your code like this, the HttpServer will load all request handler automatically.

```C#
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

```
