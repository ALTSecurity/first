using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Web;
using System.Text;
using ALTSecurity.Web.Models;

namespace ALTSecurity.Web.Models.Extraction
{
    public static class GoogleHacking
    {
        public static DorkFile LoadFile(string url, out HttpStatusCode code)
        {
            DorkFile res = new DorkFile();
            code = HttpStatusCode.OK;

            try
            {
                using (WebClient wc = new WebClient())
                {
                    res.FileData = wc.DownloadData(url);

                    string disposHeader = wc.ResponseHeaders["Content-Disposition"];
                    if (!string.IsNullOrEmpty(disposHeader))
                    {
                        string fileName = disposHeader.Substring(disposHeader.IndexOf("filename=") + 9).Replace("\"", "");
                        string withoutExt = Path.GetFileNameWithoutExtension(fileName);
                        string ext = Path.GetExtension(fileName);

                        if (Encoding.UTF8.GetByteCount(withoutExt) != withoutExt.Length)
                        {
                            byte[] value = Encoding.GetEncoding("iso-8859-1").GetBytes(withoutExt);
                            string newValue = Encoding.UTF8.GetString(value);

                            res.FileName = newValue + ext;
                        }
                        else
                        {
                            res.FileName = withoutExt + ext;
                        }
                    }
                    else
                    {
                        res.FileName = Path.GetFileName(url);
                    }
                }

                res.Url = url;
            }
            catch(WebException ex)
            {
                code = ((HttpWebResponse)ex.Response).StatusCode;
            }

            return res;
        }
    }
}