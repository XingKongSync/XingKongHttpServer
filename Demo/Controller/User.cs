using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XingKongHttpServer;

namespace Demo.Controller
{
    public class GetUserList : HttpRequestHandlerBase
    {
        public GetUserList()
        {
            Path = "/user";
        }

        public override void Handler(HttpListenerRequest request, HttpListenerResponse response)
        {
            Json(response, "{\"userList\": [\"user1\", \"user2\"]}");
        }
    }

    public class UserLogin : HttpRequestHandlerBase
    {
        private static string[] mockDbUser = { "XingKong", "Admin", "Guest" };

        public UserLogin()
        {
            Path = "/user/login";
        }

        public override void Handler(HttpListenerRequest request, HttpListenerResponse response)
        {
            string ret = "{\"code\": -1, \"result\": \"Username or password is not correct!\"}";
            string username = null;
            string password = null;

            username = request.QueryString["username"];
            password = request.QueryString["password"];

            if (mockDbUser.Where(user =>  user.Equals(username)).Count() == 1 && password.Equals("123456"))
            {
                ret = "{\"code\": 0, \"result\": \"Login success!\"}";
            }

            Json(response, ret);
        }
    }
}
