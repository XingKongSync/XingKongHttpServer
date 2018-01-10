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

        private static byte[] Http404ResponseBytes = Encoding.UTF8.GetBytes("<html><head><title>HTTP 404</title></head><body><h1>404 NOT FOUND.</h1></body></html>");
        private static byte[] Http500ResponseBytes = Encoding.UTF8.GetBytes("<html><head><title>HTTP 500</title></head><body><h1>SERVER INTERNAL ERROR.</h1></body></html>");

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

        public Dictionary<string, HttpRequestHandlerBase> PathDictionary
        {
            get
            {
                return pathDictionary;
            }
        }

        public HttpServer(int Port)
        {
            port = Port;
        }

        public void Start()
        {
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
                if (ex.ErrorCode == 5)
                {
                    Cmd cmd = new Cmd();
                    cmd.Start();
                    cmd.Input("netsh http delete urlacl url=" + prefix);
                    cmd.Input(string.Format("netsh http add urlacl url={0} user=Everyone", prefix));
                    cmd.Stop();
                }
            }
            try
            {
                listener.Start();
                startSuccess = true;
            }
            catch (Exception ex)
            {
                startSuccess = false;
                errMsg = ex.Message;
            }

            if (startSuccess)
            {
                LogData("XingKong Http Server startup succeessfully.");
                listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
            }
            else
            {
                LogData("Error: " + errMsg);
            }
        }

        public void Stop()
        {
            listener.Stop();
        }

        private void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = null;
            try
            {
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
                ThreadPool.QueueUserWorkItem(h =>
                {
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    string requestPath = request.Url.AbsolutePath;
                    if (!requestPath.EndsWith("/"))
                    {
                        requestPath += "/";
                    }
                    LogData(string.Format("http request path: {0}, Method: {1}" ,requestPath, request.HttpMethod));

                    HttpRequestHandlerBase requestHandler = null;
                    if (pathDictionary.TryGetValue(requestPath, out requestHandler) && requestHandler != null)
                    {
                        try
                        {
                            requestHandler.Handler(request, response);
                        }
                        catch (Exception ex)
                        {
                            LogData(ex.Message);
                            Default500(response);
                        }
                    }
                    else
                    {
                        Default404(response);
                    }

                    Stream output = response.OutputStream;
                    output.Close();
                });

            }
        }

        private void Default404(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            response.ContentType = "text/html";
            Stream output = response.OutputStream;
            output.Write(Http404ResponseBytes, 0, Http404ResponseBytes.Length);
        }

        private void Default500(HttpListenerResponse response)
        {
            response.StatusCode = 500;
            response.ContentType = "text/html";
            Stream output = response.OutputStream;
            output.Write(Http500ResponseBytes, 0, Http500ResponseBytes.Length);
        }

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
