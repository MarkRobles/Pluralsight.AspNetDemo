using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;

[assembly: OwinStartup(typeof(Pluralsight.AspNetDemo.Startup))]

namespace Pluralsight.AspNetDemo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Para obtener más información sobre cómo configurar la aplicación, visite https://go.microsoft.com/fwlink/?LinkID=316888

            const string connectionstring = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Pluralsight.AspNetIdentityDemo.Module2.2;Integrated Security=SSPI;";
            app.CreatePerOwinContext(() => new IdentityDbContext(connectionstring));
            app.CreatePerOwinContext<UserStore<IdentityUser>>((opt, cont) => new UserStore<IdentityUser>(cont.Get<IdentityDbContext>()));
            app.CreatePerOwinContext<UserManager<IdentityUser>>(
                (opt, cont) => new UserManager<IdentityUser>(cont.Get<UserStore<IdentityUser>>()));
        

            //Cookie stuff
            app.CreatePerOwinContext<SignInManager<IdentityUser,string>>(
                (opt, cont) =>
                new SignInManager<IdentityUser, string>(cont.Get<UserManager<IdentityUser>>(), cont.Authentication));


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }
}
