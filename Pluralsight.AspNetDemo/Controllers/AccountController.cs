﻿using Microsoft.AspNet.Identity;
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
using System.Web.UI.WebControls;

namespace Pluralsight.AspNetDemo.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<ExtendedUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<ExtendedUser>>();
        public SignInManager<ExtendedUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<ExtendedUser, string>>();


        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
         var signInStatus = await   SignInManager.PasswordSignInAsync(model.UserName, model.Password,true,true);
            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index","Home");
                              
                default:
                    ModelState.AddModelError("","Invalid Credentials");
                    return View(model);           
            }
        }

       
        public ActionResult Register() {
            return View();
        }


        [HttpPost]
        public  async Task<ActionResult> Register(RegisterModel model) {

            var user = new ExtendedUser
            {
                UserName = model.UserName,
                FullName = model.FullName

            };
            user.Addresses.Add(new Address
            {
                AddressLine = model.AddressLine,
                Country = model.Country,
                UserId = user.Id
            });

        var identityResult =  await  UserManager.CreateAsync(user, model.Password);
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