using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.Mvc;
using ALTSecurity.Web.Models;

namespace ALTSecurity.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
       // GET: Home
        public ActionResult Index()
        {
            //var model = new List<Model>();
            //model.Add(new Model { Name = "Назва", Description = "desc0" });
            return View();
        }


        //public ActionResult Modal()
        //{
        //    return PartialView("_ModalWindow");
        //}

        //public ActionResult Metadata()
        //{
        //    return PartialView("_Metadata");
        //}

        //public ActionResult Cve()
        //{
        //    List<Cve> res = new List<Cve>
        //    {
        //        new Cve
        //        {
        //            Name = "OpenSSH 7.4",
        //            CveItem = "CVE-2016-10708"
        //        },
        //        new Cve
        //        {
        //            Name = "Exim smtpd 4.93",
        //            CveItem = "CVE-2020-12783"
        //        },
        //        new Cve
        //        {
        //            Name = "MySQL 5.5.5-10.1.41-MariaDB",
        //            CveItem = "CVE-2020-8611"
        //        }
        //    };
        //    return PartialView("_CveList", res);
        //}
    }
}