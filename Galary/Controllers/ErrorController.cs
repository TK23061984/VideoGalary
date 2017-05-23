using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Galary.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ViewResult AccessDenied()
        {
            return View();
        }
    }
}