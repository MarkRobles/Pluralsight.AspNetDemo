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
               
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("ChooseProvider");
                              
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