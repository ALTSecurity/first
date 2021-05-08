using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Net;

namespace ALTSecurity.Web.Models.Extraction
{
    public class DnsEnumeration
    {
        readonly string ApiZoneUrl = "https://api.hackertarget.com/zonetransfer/?q={0}";

        /// <summary>
        /// Отримати перелік піддоменів
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public Dictionary<string,DomainState> RecieveIps(string domain)
        {
            Dictionary<string,DomainState> res = new Dictionary<string, DomainState>();
            string response = string.Empty;

            using (WebClient wc = new WebClient())
            {
                response = wc.DownloadString(String.Format(ApiZoneUrl, domain));
            }

            Regex domainRegex = new Regex(@"(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]");
            List<string> dom = domainRegex.Matches(response)
                .Cast<Match>()
                .Select(x => x.Value)
                .Distinct()
                .Where(x => x.Contains(domain) && x != domain)
                .ToList();

            int count = 0;
            string ip = string.Empty;
            foreach (var item in dom)
            {
                if (count >= 20)
                {
                    break;
                }

                ip = item.ToString();
                if (res.ContainsKey(ip))
                {
                    continue;
                }

                if (!TestConnection(ip, ConnectionType.http, null))
                {
                    res.Add(ip, DomainState.closed);
                }
                else if (!TestConnection(ip, ConnectionType.https, null))
                {
                    res.Add(ip, DomainState.notSecured);
                }
                else
                {
                    res.Add(ip, DomainState.open);
                }

                count++;
            }
            return res;
        }

        /// <summary>
        /// Перевірити доступність по http та https протоколу
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="type"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public bool TestConnection(string domain, ConnectionType type, WebProxy proxy = null)
        {
            bool available = true;
            string url = (type == ConnectionType.http ? "http" : "https") + "://" + domain;
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "HEAD";
                if (proxy != null)
                {
                    req.Proxy = proxy;
                }

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException)
            {
                available = false;
            }

            return available;
        }
    }

    public enum ConnectionType
    {
        http = 0,
        https = 1
    }

    public enum DomainState
    {
        open = 0,
        notSecured = 1,
        closed = 2
    }
}