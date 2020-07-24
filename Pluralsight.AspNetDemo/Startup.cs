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
            app.CreatePerOwinContext<UserManager<ExtendedUser>>(
                (opt, cont) => new UserManager<ExtendedUser>(cont.Get<UserStore<ExtendedUser>>()));
        

            //Cookie stuff
            app.CreatePerOwinContext<SignInManager<ExtendedUser,string>>(
                (opt, cont) =>
                new SignInManager<ExtendedUser, string>(cont.Get<UserManager<ExtendedUser>>(), cont.Authentication));


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }
}
