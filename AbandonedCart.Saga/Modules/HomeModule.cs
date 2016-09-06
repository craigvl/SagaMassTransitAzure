using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Owin;

namespace Abt.Result.WebApi.Modules
{
    public class HomeModule : NancyModule
    {

        public HomeModule()
        {
            Get["/"] = x =>
            {
                var env = this.Context.GetOwinEnvironment();

                var requestBody = (Stream) env["owin.RequestBody"];
                var requestHeaders = (IDictionary<string, string[]>) env["owin.RequestHeaders"];
                var requestMethod = (string) env["owin.RequestMethod"];
                var requestPath = (string) env["owin.RequestPath"];
                var requestPathBase = (string) env["owin.RequestPathBase"];
                var requestProtocol = (string) env["owin.RequestProtocol"];
                var requestQueryString = (string) env["owin.RequestQueryString"];
                var requestScheme = (string) env["owin.RequestScheme"];

                var responseBody = (Stream) env["owin.ResponseBody"];
                var responseHeaders = (IDictionary<string, string[]>) env["owin.ResponseHeaders"];

                var owinVersion = (string) env["owin.Version"];
                var cancellationToken = (CancellationToken) env["owin.CallCancelled"];

                var uri = (string) env["owin.RequestScheme"] + "://" + requestHeaders["Host"].First() +
                          (string) env["owin.RequestPathBase"] + (string) env["owin.RequestPath"];

                if (env["owin.RequestQueryString"] != string.Empty)
                {
                    uri += "?" + (string)env["owin.RequestQueryString"];
                }

                return $"{requestMethod} {uri}";
            };
        }
    }
}
