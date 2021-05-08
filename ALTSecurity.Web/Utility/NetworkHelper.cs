using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using HtmlAgilityPack;

namespace ALTSecurity.Web.Utility
{
    public static class NetworkHelper
    {
        public static List<string> ParseGoogle(string query, int startpage, int lastpage)
        {
            List<string> Urls = new List<string>();
            for (var count = startpage - 1; count < lastpage - 1; count++)
            {
                string url = "https://www.google.com.ua/search?q=" + query + "&start=" + (count * 10 + 1);
                HtmlDocument doc = new HtmlDocument();

                using (WebClient wc = new WebClient())
                {
                    doc.LoadHtml(wc.DownloadString(url));
                }

                IEnumerable<HtmlNode> results;
                if (doc != null)
                {
                    results = doc.DocumentNode.Descendants().Where(x => x.Attributes.Any(a => a.Value.Contains("/url?q=")));
                    foreach (var item in results)
                    {
                        string u = item.Attributes["href"].Value;
                        if (u.Contains("ru") || u.Contains("by") || u.Contains("wiki") || u.Contains("youtube"))
                        {
                            continue;
                        }
                        u = u.Substring(u.IndexOf("url?q=") + 6);
                        Urls.Add(u.Remove(u.IndexOf("&amp")));
                    }
                }
            }
            return Urls;
        }
    }
}