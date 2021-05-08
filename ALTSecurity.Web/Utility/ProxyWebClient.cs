using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace ALTSecurity.Web.Utility
{
    public class ProxyWebClient: WebClient
    {
        WebProxy _proxy { get; set; }

        int _timeout { get; set; }

        public ProxyWebClient(WebProxy proxy)
        {
            _timeout = 100000;
            _proxy = proxy;
        }

        public ProxyWebClient(WebProxy proxy, int timeout)
        {
            _timeout = timeout;
            _proxy = proxy;
        }


        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            request.Timeout = _timeout;

            request.Proxy = _proxy;

            return request;
        }
    }
}