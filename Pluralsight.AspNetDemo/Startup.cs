using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Pluralsight.AspNetDemo.DAL;
using Pluralsight.AspNetDemo.Models;
using Pluralsight.AspNetDemo.Services;

[assembly: OwinStartup(typeof(Pluralsight.AspNetDemo.Startup))]

namespace Pluralsight.AspNetDemo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Para obtener más información sobre cómo configurar la aplicación, visite https://go.microsoft.com/fwlink/?LinkID=316888

            const string connectionstring = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Pluralsight.AspNetIdentityDemo.Module2.2;Integrated Security=SSPI;";
            app.CreatePerOwinContext(() => new ExtendedUserDbContext(connectionstring));
            app.CreatePerOwinContext<UserStore<ExtendedUser>>((opt, cont) => new UserStore<ExtendedUser>(cont.Get<ExtendedUserDbContext>()));
           //I modify this for 2FV
            app.CreatePerOwinContext<UserManager<ExtendedUser>>(
                (opt, cont) =>
                {
                    var usermanager = new UserManager<ExtendedUser>(cont.Get<UserStore<ExtendedUser>>());
                    usermanager.RegisterTwoFactorProvider("SMS", new PhoneNumberTokenProvider<ExtendedUser>() { MessageFormat ="Token{0}"});
                    usermanager.SmsService = new SmsService();
                    return usermanager;
                });
        

            //Cookie stuff
            app.CreatePerOwinContext<SignInManager<ExtendedUser,string>>(
                (opt, cont) =>
                new SignInManager<ExtendedUser, string>(cont.Get<UserManager<ExtendedUser>>(), cont.Authentication));


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });

            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }
    }
}
