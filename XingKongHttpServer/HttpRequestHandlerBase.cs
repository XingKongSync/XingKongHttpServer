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
        /// 返回文本数据
        /// </summary>
        /// <param name="response"></param>
        /// <param name="text"></param>
        protected void Text(HttpListenerResponse response, string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            response.ContentType = "text/plain";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 返回Json数据
        /// </summary>
        /// <param name="response"></param>
        /// <param name="json"></param>
        protected void Json(HttpListenerResponse response, string json)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 返回Html数据
        /// </summary>
        /// <param name="response"></param>
        /// <param name="html"></param>
        protected void Html(HttpListenerResponse response, string html)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(html);
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
        }
    }
}
