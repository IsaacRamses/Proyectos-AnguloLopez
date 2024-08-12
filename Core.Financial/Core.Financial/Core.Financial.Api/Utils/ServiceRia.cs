using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Core.Financial.Api.Utils
{
    public class ServiceRia
    {
        public HttpClient HeaderRia(string pin)
        {
            HttpClient Client = new HttpClient();
            var AgentId = ConfigurationManager.AppSettings["ria-AgentId"];
            var Key = ConfigurationManager.AppSettings["Ocp-Apim-Subscription-Key"];
            var ApiVersion = ConfigurationManager.AppSettings["ria-ApiVersion"];

            var today = DateTime.Now.ToString("yyyyMMddHHmmss");
            Client.DefaultRequestHeaders.Add("ria-CallerCorrelationId", pin);
            Client.DefaultRequestHeaders.Add("ria-CallDateTimeLocal", today);
            Client.DefaultRequestHeaders.Add("ria-CallerUserId", "");
            Client.DefaultRequestHeaders.Add("ria-CallerDeviceId", "");
            Client.DefaultRequestHeaders.Add("ria-AgentId", AgentId);
            Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Key);
            Client.DefaultRequestHeaders.Add("ria-ApiVersion", ApiVersion);

            return Client;
        }

        public HttpWebRequest ConexionRIA(string pin, string url, string Method)
        {
            var AgentId = ConfigurationManager.AppSettings["ria-AgentId"];
            var Key = ConfigurationManager.AppSettings["Ocp-Apim-Subscription-Key"];
            var ApiVersion = ConfigurationManager.AppSettings["ria-ApiVersion"];
            var domain = ConfigurationManager.AppSettings["UrlRia"];
            var today = DateTime.Now.ToString("yyyyMMddHHmmss");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Concat(domain, url));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = Method;
            httpWebRequest.Headers.Add("ria-CallerCorrelationId", pin);
            httpWebRequest.Headers.Add("ria-CallDateTimeLocal", today);
            httpWebRequest.Headers.Add("ria-CallerUserId", "");
            httpWebRequest.Headers.Add("ria-CallerDeviceId", "");
            httpWebRequest.Headers.Add("ria-AgentId", AgentId);
            httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", Key);
            httpWebRequest.Headers.Add("ria-ApiVersion", ApiVersion);

            return httpWebRequest;
        }
    }
}