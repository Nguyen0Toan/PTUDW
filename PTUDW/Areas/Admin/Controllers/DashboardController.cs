using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTUDW.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Admin/DashboardDefault
        public ActionResult Index()
        {
            return View();
        }
    }
}