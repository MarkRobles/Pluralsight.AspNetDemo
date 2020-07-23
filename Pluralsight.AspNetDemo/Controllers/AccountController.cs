using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Pluralsight.AspNetDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Pluralsight.AspNetDemo.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<IdentityUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();



        public ActionResult Register() {
            return View();
        }


        [HttpPost]
        public  async Task<ActionResult> Register(RegisterModel model) {


        var identityResult =  await  UserManager.CreateAsync(new IdentityUser(model.UserName), model.Password);
            if (identityResult.Succeeded) {
                return RedirectToAction("Index","Home");
            }

            ModelState.AddModelError("", identityResult.Errors.FirstOrDefault());
            return View(model);
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
    }
}