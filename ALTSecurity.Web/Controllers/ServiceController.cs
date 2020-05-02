using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using ALTSecurity.Web.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ALTSecurity.Web.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Service
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetResources()
        {
            var globalObj = new JObject();
            var globalRes = Resources.Global.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            IDictionaryEnumerator globalEnum = globalRes.GetEnumerator();
            while (globalEnum.MoveNext())
            {
                globalObj.Add(globalEnum.Key.ToString(), globalEnum.Value.ToString());
            }

            var messagesObj = new JObject();
            var messagesRes = Resources.Messages.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            IDictionaryEnumerator messagesEnum = messagesRes.GetEnumerator();
            while (messagesEnum.MoveNext())
            {
                messagesObj.Add(messagesEnum.Key.ToString(), messagesEnum.Value.ToString());
            }

            string res = @"window.Resources = {
                             Global: " + JsonConvert.SerializeObject(globalObj) + @",
                             Messages: " + JsonConvert.SerializeObject(messagesObj) + @"
                          };";

            return JavaScript(res);
        }
    }
}