using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Pluralsight.AspNetDemo.DAL;
using Pluralsight.AspNetDemo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Pluralsight.AspNetDemo.Controllers
{
    public class UserController : Controller
    {

        public UserManager<ExtendedUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<ExtendedUser>>();

        public ActionResult Index()
        {
            return View(UserManager.Users.ToList());
        }



        public async  Task<ActionResult> Details(string id) {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                TempData["Message"] = "User Not Found";
                return View("Error");
            }
        }

        public async Task<ActionResult> Edit(string id) {

            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                TempData["Message"] = "User Not Found";
                return View("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ExtendedUser model) {
            try
            {
                var status = await UserManager.UpdateAsync(model);
                if (status.Succeeded)
                {
                    TempData["Message"] = String.Concat(model.UserName, " updated succesfully");
                    return RedirectToAction("Details", model.Id);
                }
                else
                {
                    TempData["Message"] = status.Errors.ToString();
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return View("Error");
            }
           
        }


        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                TempData["Message"] = "User Not Found";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, FormCollection form)
        {
            var user = await UserManager.FindByIdAsync(id);
            var status = await UserManager.DeleteAsync(user);
            if (status.Succeeded)
            {
                TempData["Message"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = status.Errors.ToString();
                return View("Error");
            }
        }


    }
}