using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XingKongHttpServer
{
    static class ControllerLoader
    {
        internal static IEnumerable<HttpRequestHandlerBase> LoadControllers(Type Module, string controllerNamespace)
        {
            List<HttpRequestHandlerBase> ret = new List<HttpRequestHandlerBase>();
            if (Module != null && !string.IsNullOrWhiteSpace(controllerNamespace))
            {
                IEnumerable<Type> controllers = Module.Assembly.GetTypes().Where(type => (type.FullName.StartsWith(controllerNamespace) && type.IsSubclassOf(typeof(HttpRequestHandlerBase))));
                foreach (Type type in controllers)
                {
                    object instance = type.Assembly.CreateInstance(type.FullName);
                    HttpRequestHandlerBase action = instance as HttpRequestHandlerBase;
                    if (action != null)
                    {
                        ret.Add(action);
                    }
                }
            }
            return ret;
        }
    }
}
