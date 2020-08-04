using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Pluralsight.AspNetDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pluralsight.AspNetDemo.DAL
{
    public class ApplicationRoleManager : RoleManager<ExtentedRole>
    {
        public ApplicationRoleManager(IRoleStore<ExtentedRole, string> roleStore)
            : base(roleStore)
        {
        }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<ExtentedRole>(context.Get<ExtendedUserDbContext>()));
            
            
            
            return manager;
        }
    }
}