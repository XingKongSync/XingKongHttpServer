using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XingKongHttpServer
{
    public class HttpServer
    {
        private int port;
        private HttpListener listener = new HttpListener();
        private Dictionary<string, HttpRequestHandlerBase> pathDictionary = new Dictionary<string, HttpRequestHandlerBase>();

        //HTTP 404 的返回正文
        private static byte[] Http404ResponseBytes = Encoding.UTF8.GetBytes("<html><head><title>HTTP 404</title></head><body><h1>404 NOT FOUND.</h1></body></html>");
        //HTTP 500 的返回正文
        private static byte[] Http500ResponseBytes = Encoding.UTF8.GetBytes("<html><head><title>HTTP 500</title></head><body><h1>SERVER INTERNAL ERROR.</h1></body></html>");

        /// <summary>
        /// 获取HttpServer监听的端口号
        /// </summary>
        public int Port
        {
            get
            {
                if (port == 0)
                {
                    throw new ArgumentNullException("Port未设置");
                }
                return port;
            }
        }

        /// <summary>
        /// 路由与请求处理器的映射
        /// </summary>
        public Dictionary<string, HttpRequestHandlerBase> PathDictionary
        {
            get
            {
                return pathDictionary;
            }
        }

        /// <summary>
        /// 创建一个HttpServer实例
        /// </summary>
        /// <param name="Port">端口号</param>
        public HttpServer(int Port)
        {
            port = Port;
        }

        /// <summary>
        /// 启动Http服务
        /// </summary>
        public void Start()
        {
            //监听本机所有网卡的该端口
            string prefix = string.Format("http://*:{0}/", Port);

            listener.Prefixes.Clear();
            listener.Prefixes.Add(prefix);

            bool startSuccess = false;
            string errMsg = string.Empty;
            try
            {
                listener.Start();
                startSuccess = true;
            }
            catch (HttpListenerException ex)
            {
                //发生该错误通常是由于未使用管理员身份运行导致的
                if (ex.ErrorCode == 5)
                {
                    //尝试在系统中绑定HTTP协议
                    Cmd cmd = new Cmd();
                    cmd.Start();
                    cmd.Input("netsh http delete urlacl url=" + prefix);
                    cmd.Input(string.Format("netsh http add urlacl url={0} user=Everyone", prefix));
                    cmd.Stop();
                }
                //重试启动Http服务
                try
                {
                    listener.Start();
                    startSuccess = true;
                }
                catch (Exception ex2)
                {
                    startSuccess = false;
                    errMsg = ex2.Message;
                }
            }

            if (startSuccess)
            {
                //启动Http服务成功
                LogData("XingKong Http Server startup succeessfully.");
                //开始等待用户请求
                listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
            }
            else
            {
                //启动Http服务失败，输出错误原因
                LogData("Error: " + errMsg);
            }
        }

        public void Stop()
        {
            listener.Stop();
        }

        /// <summary>
        /// 用户发起请求后的回调
        /// </summary>
        /// <param name="result">HttpListener</param>
        private void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = null;
            try
            {
                //尝试读取用户请求
                context = listener.EndGetContext(result);
            }
            catch (Exception ex)
            {
                LogData(ex.Message);
            }
            if (listener.IsListening)
            {
                listener.BeginGetContext(ListenerCallback, listener);
            }
            if (context != null)
            {
                //读取用户请求成功，放入线程池中执行
                ThreadPool.QueueUserWorkItem(h =>
                {
                    HttpListenerRequest request = context.Request; //用户请求
                    HttpListenerResponse response = context.Response; //请求响应结果

                    //获取用户请求路径
                    string requestPath = request.Url.AbsolutePath;
                    if (!requestPath.EndsWith("/"))
                    {
                        requestPath += "/";
                    }
                    LogData(string.Format("http request path: {0}, Method: {1}" ,requestPath, request.HttpMethod));

                    //对请求路径进行匹配
                    HttpRequestHandlerBase requestHandler = null;
                    if (pathDictionary.TryGetValue(requestPath, out requestHandler) && requestHandler != null)
                    {
                        //路径匹配成功并且成功找到请求处理器
                        try
                        {
                            //开始处理请求
                            requestHandler.Handler(request, response);
                        }
                        catch (Exception ex)
                        {
                            //处理请求时出错
                            LogData(ex.Message);
                            //返回HTTP 500错误
                            Default500(response);
                        }
                    }
                    else
                    {
                        //无法匹配请求处理器
                        //返回HTTP 404错误
                        Default404(response);
                    }
                    //关闭输出流
                    Stream output = response.OutputStream;
                    output.Close();
                });

            }
        }

        /// <summary>
        /// 返回HTTP 404错误
        /// </summary>
        /// <param name="response"></param>
        private void Default404(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            response.ContentType = "text/html";
            Stream output = response.OutputStream;
            output.Write(Http404ResponseBytes, 0, Http404ResponseBytes.Length);
        }

        /// <summary>
        /// 返回HTTP500 错误
        /// </summary>
        /// <param name="response"></param>
        private void Default500(HttpListenerResponse response)
        {
            response.StatusCode = 500;
            response.ContentType = "text/html";
            Stream output = response.OutputStream;
            output.Write(Http500ResponseBytes, 0, Http500ResponseBytes.Length);
        }

        /// <summary>
        /// 添加一个路由及请求处理器
        /// </summary>
        /// <param name="handler"></param>
        public void AddPath(HttpRequestHandlerBase handler)
        {
            PathDictionary.Add(handler.Path, handler);
        }

        /// <summary>
        /// 载入一个命名空间下的全部Controller
        /// 遍历这个命名空间下的全部HttpRequestHandlerBase子类，并实例化，
        /// </summary>
        /// <param name="Module"></param>
        /// <param name="controllerNamespace"></param>
        public void LoadController(Type Module, string controllerNamespace)
        {
            IEnumerable<HttpRequestHandlerBase> handlers = ControllerLoader.LoadControllers(Module, controllerNamespace);
            foreach (HttpRequestHandlerBase handler in handlers)
            {
                AddPath(handler);
            }
        }

        private void LogData(string data = "")
        {
            Console.Write("[HttpServer]");
            Console.WriteLine(data);
        }
    }
}
