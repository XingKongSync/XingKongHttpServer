using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XingKongHttpServer
{
    public abstract class HttpRequestHandlerBase
    {
        private string path;

        /// <summary>
        /// 路由路径
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }

            protected set
            {
                if (!value.EndsWith("/"))
                {
                    value += "/";
                }
                path = value;
            }
        }

        /// <summary>
        /// 请求处理函数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public abstract void Handler(HttpListenerRequest request, HttpListenerResponse response);

        /// <summary>
        /// 返回HTTP数据
        /// </summary>
        /// <param name="contentType">默认text/html</param>
        /// <param name="encoding">默认UTF-8</param>
        /// <param name="data"></param>
        protected void CreateResponse(HttpListenerResponse response, byte[] data, string contentType = "text/html", Encoding encoding = null)
        {
            response.ContentType = contentType;
            response.ContentEncoding = encoding ?? Encoding.UTF8;
            response.ContentLength64 = data.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(data, 0, data.Length);
        }

        /// <summary>
        /// 返回文本数据
        /// </summary>
        /// <param name="response"></param>
        /// <param name="text"></param>
        protected void Text(HttpListenerResponse response, string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            CreateResponse(response, buffer, "text/plain");
        }

        /// <summary>
        /// 返回Json数据
        /// </summary>
        /// <param name="response"></param>
        /// <param name="json"></param>
        protected void Json(HttpListenerResponse response, string json)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            CreateResponse(response, buffer, "application/json");
        }

        /// <summary>
        /// 返回Html数据
        /// </summary>
        /// <param name="response"></param>
        /// <param name="html"></param>
        protected void Html(HttpListenerResponse response, string html)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(html);
            CreateResponse(response, buffer, "text/html");
        }
    }
}
