﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Pluralsight.AspNetDemo.DAL;
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
        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().Get<ApplicationUserManager>();
        public SignInManager<ExtendedUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<ExtendedUser, string>>();

        private ApplicationRoleManager _roleManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }

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
               
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("ChooseProvider");
                case SignInStatus.LockedOut:
                    var user = await UserManager.FindByNameAsync(model.UserName);

                    if (user != null)
                    {
                    
                        await UserManager.SendEmailAsync(user.Id, "LockoutEmail", $"Hi  {user.UserName}, You have been locked out due to repeated, failed login attemps, Please try to reset your password");
                    }
                    return RedirectToAction("Index", "Home");
                default:
                    ModelState.AddModelError("","Invalid Credentials");
                    return View(model);           
            }
        }

        public async Task<ActionResult> ChooseProvider() {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            var providers = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            return View(new ChooseProviderModel { Providers = providers.ToList() });
        }

        [HttpPost]
        public async Task<ActionResult> ChooseProvider(ChooseProviderModel model)
        {
            await SignInManager.SendTwoFactorCodeAsync(model.ChosenProvider);
            return RedirectToAction("TwoFactor", new {  provider = model.ChosenProvider});
        }

        public ActionResult TwoFactor(string provider)
        {
            return View(new TwoFactorModel { Provider = provider });
        }

        [HttpPost]
        public async  Task<ActionResult> TwoFactor(TwoFactorModel model)
        {
         var signInStatus =    await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code,true,model.RememberBrowser);
            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index","Home");
                default:
                    ModelState.AddModelError("","Invalid Credentials");
                    return View(model);
            }
        }

        [Authorize(Roles = "IT")]
        public ActionResult Register() {
            CreateDropDownRoles();

            return View();
        }


        public void CreateDropDownRoles()
        {
            var roles = _roleManager.Roles.ToList();

            List<SelectListItem> dropDownItems = new List<SelectListItem>();
            foreach (var role in roles)
            {
                SelectListItem item = new SelectListItem { Value = role.Id, Text = role.Name };
                dropDownItems.Add(item);
            }
            ViewBag.Roles = dropDownItems;

        }

        [Authorize(Roles = "IT")]
        [HttpPost]
        public  async Task<ActionResult> Register(RegisterModel model) {


            if(ModelState.IsValid)
            {
                var result = await UserManager.PasswordValidator.ValidateAsync(model.Password);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.FirstOrDefault());
                    return View(model);
                }

                var user = new ExtendedUser
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Email = model.UserName

                };
                user.Addresses.Add(new Address
                {
                    AddressLine = model.AddressLine,
                    Country = model.Country,
                    UserId = user.Id
                });


                var identityResult = await UserManager.CreateAsync(user, model.Password);
                if (identityResult.Succeeded)
                {

                    await AddRoleTouser(user.Id, model.role);


                    var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var confirmUrl = Url.Action("ConfirmEmail", "Account", new { userid = user.Id, token = token }, Request.Url.Scheme);

                    await UserManager.SendEmailAsync(user.Id, "Email Confirmation", $"Use link to confirm email: {confirmUrl}");
                    TempData["Message"] = string.Concat(model.UserName," created successfully");
                    return RedirectToAction("Index", "uSER");
                }
                ModelState.AddModelError("", identityResult.Errors.FirstOrDefault());
            }

            return View(model);
        }


        public async Task<ActionResult> AddRoleTouser(string Id, string role)
        {
            try
            {
                //role = "User";
                var roles = await UserManager.GetRolesAsync(Id);
                if (roles.Any())
                {
                    TempData["Message"] = "Esta aplicacion solo permite un rol por usuario";
                }

            var  result =     await UserManager.AddToRoleAsync(Id, role);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return View("Error");
            }

        }

        public async Task<ActionResult> ConfirmEmail(string userid, string token)
        {
            var identityResult = await UserManager.ConfirmEmailAsync(userid, token);
            if (!identityResult.Succeeded)
            {
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {

            var user = await UserManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var resetUrl = Url.Action("PasswordReset", "Account", new { userid = user.Id, token = token }, Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Password Reset", $"Use link to reset password {resetUrl}");
            }

            return RedirectToAction("Index", "Home");
        }


        public ActionResult PasswordReset(string userid, string token)
        {
            return View(new PasswordResetModel { UserId = userid, Token = token });
        }

        [HttpPost]
        public async Task<ActionResult> PasswordReset(PasswordResetModel model)
        {
            var identityResult = await UserManager.ResetPasswordAsync(model.UserId, model.Token, model.Password);
            if (!identityResult.Succeeded)
            {
                return View("Error");
            }
            return RedirectToAction("Index", "Home");

        }

        public ActionResult ExternalAuthentication(string provider)
        {
            SignInManager.AuthenticationManager.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("ExternalCallback", new { provider })
                }, provider);
            return new HttpUnauthorizedResult();

        }

        public async Task<ActionResult> ExternalCallback(string provider)
        {
            var loginInfo = await SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();
            var signInStatus = await SignInManager.ExternalSignInAsync(loginInfo, true);

            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");

                default:
                    var user = await UserManager.FindByEmailAsync(loginInfo.Email);
                    if (user != null)
                    {
                        var result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                        if (result.Succeeded)
                        {
                            return await ExternalCallback(provider);
                        }
                    }
                    return View("Error");

            }
        }
    }
}