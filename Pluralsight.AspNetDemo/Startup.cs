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
using Microsoft.Owin.Security.Google;
using System.Configuration;

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
                    usermanager.UserTokenProvider = new DataProtectorTokenProvider<ExtendedUser>(opt.DataProtectionProvider.Create());
                    usermanager.EmailService = new EmailService();

                    usermanager.UserValidator = new UserValidator<ExtendedUser>(usermanager){RequireUniqueEmail = true };
                    usermanager.PasswordValidator = new PasswordValidator
                    {
                        RequireDigit = true,
                        RequiredLength = 8,
                        RequireLowercase = true,
                        RequireNonLetterOrDigit = true,
                        RequireUppercase = true
                    };

                    //Configure User lock   Out
                    usermanager.UserLockoutEnabledByDefault = true;
                    usermanager.MaxFailedAccessAttemptsBeforeLockout = 2;
                    usermanager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(3);
                    return usermanager;
                });
        

            //Cookie stuff
            app.CreatePerOwinContext<SignInManager<ExtendedUser,string>>(
                (opt, cont) =>
                new SignInManager<ExtendedUser, string>(cont.Get<UserManager<ExtendedUser>>(), cont.Authentication));


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager<ExtendedUser, string>, ExtendedUser>(
                    validateInterval: TimeSpan.FromSeconds(3),//change to 30 minutes or 1 hour after test based on your cookie lifetime
                    regenerateIdentity: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });

            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);


            //if this external signin cookie is after the google authenticator middleware(below) you will get an error
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //Configure google  authentication 
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["google:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["google:ClientSecret"],
                Caption = "Google"
            }); ;




        }
    }
}
