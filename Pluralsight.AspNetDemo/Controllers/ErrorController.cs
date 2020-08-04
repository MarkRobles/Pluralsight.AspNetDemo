using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pluralsight.AspNetDemo.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ViewResult Unauthorized()
        {
            //Response.StatusCode = 401; // Do not set this or else you get a redirect loop
            return View();
        }
    }
}