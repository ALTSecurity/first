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
            var model = new List<Model>();
            model.Add(new Model { Name = "Назва", Description = "desc0" });
            return View(model);
        }


        public ActionResult Modal()
        {
            return PartialView("_ModalWindow");
        }
    }
}