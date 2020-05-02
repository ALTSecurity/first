using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALTSecurity.Web.Utility
{
    public class JsonResponse : JsonResult
    {
        public JsonResponse()
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }

        public JsonResponse(int code)
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Data = new { code = code };
        }

        public JsonResponse(int code, string msg)
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Data = new { code = code, msg = msg };
        }

        public JsonResponse(int code, string msg, object data)
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Data = new { code = code, msg = msg, data = data };
        }

        public JsonResponse(object data)
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Data = data;
        }
    }
}