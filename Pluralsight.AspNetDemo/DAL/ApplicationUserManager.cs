using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Pluralsight.AspNetDemo.Models;
using Pluralsight.AspNetDemo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pluralsight.AspNetDemo.DAL
{
    public class ApplicationUserManager : UserManager<ExtendedUser>
    {
        public ApplicationUserManager(IUserStore<ExtendedUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext cont)
        {
            var usermanager = new ApplicationUserManager(new UserStore<ExtendedUser>(cont.Get<ExtendedUserDbContext>()));
           // var usermanager = new ApplicationUserManager<ExtendedUser>(cont.Get<UserStore<ExtendedUser>>());
            usermanager.RegisterTwoFactorProvider("SMS", new PhoneNumberTokenProvider<ExtendedUser>() { MessageFormat = "Token{0}" });
            usermanager.SmsService = new SmsService();
            usermanager.UserTokenProvider = new DataProtectorTokenProvider<ExtendedUser>(options.DataProtectionProvider.Create());
            usermanager.EmailService = new EmailService();

            usermanager.UserValidator = new UserValidator<ExtendedUser>(usermanager) { RequireUniqueEmail = true };
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
        }


        public override Task<IdentityResult> AddToRoleAsync(string userId, string roleId)
        {
            try
            {
                using (var ctx = new ExtendedUserDbContext())
                {
                    ctx.ApplicationUserRole.Add(new ApplicationUserRole
                    {
                        UserId = userId,
                        RoleId = roleId
                    });

                    ctx.SaveChanges();
                }
                return Task.FromResult(new IdentityResult() { });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new IdentityResult(ex.Message));
            }
        }

    }
}