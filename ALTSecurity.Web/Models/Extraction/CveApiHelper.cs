using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ALTSecurity.Web.Models.Extraction
{
    [Serializable]
    public class CveMetadata
    {
        public string id { get; set; }

        public string assigner { get; set; }
    }

    [Serializable]
    public class CveReferenceData
    {
        public string url { get; set; }

        public string name { get; set; }

        public string refsource { get; set; }

        public List<string> tags { get; set; }
    }

    [Serializable]
    public class CveReference
    {
        [JsonProperty("reference_data")]
        public List<CveReferenceData> reference_data { get; set; }
    }

    /// <summary>
    /// Клас опису вразливості
    /// </summary>
    [Serializable]
    public class Cve
    {
        public string data_type { get; set; }

        public string data_format { get; set; }

        public string data_version { get; set; }

        [JsonProperty("CVE_data_meta")]
        public CveMetadata metadata { get; set; }

        public CveReference references { get; set; }

    }

    [Serializable]
    public class CveItem
    {
        public Cve cve { get; set; }

        public DateTime publishedDate { get; set; }

        public DateTime lastModifiedDate { get; set; }
    }

    public static class CveApiHelper
    {
        readonly static string ApiUrl = "https://services.nvd.nist.gov/rest/json/cves/1.0";

        public static List<CveItem> GetVulnData(CveApiQuery builder)
        {
            List<CveItem> res = new List<CveItem>();
            string query = string.Empty;

            switch (builder.SearchType)
            {
                case ApiSearchType.keyword:
                    query = String.Format("{0}?keyword={1}", ApiUrl, builder.SoftName + " " + builder.SoftVersion);
                    break;
                case ApiSearchType.matchString:
                    query = String.Format("{0}?cpeMatchString=cpe:2.3:o:{1}:{2}", ApiUrl, builder.SoftName, builder.SoftVersion);
                    break;
                default:
                    break;
            }

            using (WebClient wc = new WebClient())
            {
                string response = wc.DownloadString(query);

                if (response.Contains("CVE_Items"))
                {
                    dynamic jsonData = JObject.Parse(response)["result"]["CVE_Items"];
                    res = JsonConvert.DeserializeObject<List<CveItem>>(Convert.ToString(jsonData));
                }
            }

            return res;
        }
    }

    /// <summary>
    /// Клас для побудови запиту на API
    /// </summary>
    public class CveApiQuery
    {
        public string SoftName { get; set; }

        public string SoftVersion { get; set; }

        public ApiSearchType SearchType { get; set; }
    }

    /// <summary>
    /// Тип пошуку
    /// </summary>
    public enum ApiSearchType
    {
        keyword = 0,
        matchString = 1
    }
}