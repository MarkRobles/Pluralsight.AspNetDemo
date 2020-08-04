using Microsoft.Ajax.Utilities;
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
    [Authorize(Roles ="IT")]
    public class RoleController : Controller
    {

        private ApplicationRoleManager _roleManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }
        public ActionResult Index()
        {
            return View(_roleManager.Roles.ToList());
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ExtendedRole model)
        {
            var result = await _roleManager.CreateAsync(model);
            if (result.Succeeded)
            {
                TempData["Message"] = "Created Successfully";
                return RedirectToAction("Details", new { Id = model.Id });
            }
            else
            {
                TempData["Message"] = result.Errors.ToString();
                return View("Error");
            }
        }


        public async Task<ActionResult> Details(string id)
        {
           
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    return View(role);
                }
                else
                {
                    TempData["Message"] = "Roles Not Found";
                    return View("Error");
                }
            
        }

        public async Task<ActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                return View(role);
            }
            else
            {
                TempData["Message"] = "Role Not Found";
                return View("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ExtendedRole model)
        {
            var status = await _roleManager.UpdateAsync(model);
            if (status.Succeeded)
            {
                TempData["Message"] = "Updated Successfully";
                return RedirectToAction("Details", new { Id = model.Id });
            }
            else
            {
                TempData["Message"] = status.Errors.ToString();
                return View("Error");
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                return View(role);
            }
            else
            {
                TempData["Message"] = "Role Not Found";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, FormCollection form)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var status = await _roleManager.DeleteAsync(role);
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