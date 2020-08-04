namespace Pluralsight.AspNetDemo.Migrations
{
    using Antlr.Runtime.Tree;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Pluralsight.AspNetDemo.DAL;
    using Pluralsight.AspNetDemo.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Pluralsight.AspNetDemo.DAL.ExtendedUserDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected  override void Seed(Pluralsight.AspNetDemo.DAL.ExtendedUserDbContext context)
        {
            ApplicationUserManager UserManager = new ApplicationUserManager(new UserStore<ExtendedUser>(context));

            var ItRole = new ExtendedRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "IT",
                Details = "Super admin"
            };




            context.Roles.AddOrUpdate(ItRole);
            context.SaveChanges();
            //Modify this after  download from github
            var password = UserManager.PasswordHasher.HashPassword("Password_2020");
          
                var UserAdmin = new ExtendedUser
                {
                    UserName = "user@gmail.com",
                    Email = "user@gmail.com",
                    FullName = "Nombres Apellidos",
                    PhoneNumber = "+0000000000",
               PasswordHash= password,
                    TwoFactorEnabled = true,
                    EmailConfirmed = false,
                    LockoutEnabled = true
                };

            UserManager.Create(UserAdmin);
     

      
                context.SaveChanges();

                var userRole = new ApplicationUserRole
                {
                    RoleId = ItRole.Id,
                    UserId = UserAdmin.Id
                };
                context.ApplicationUserRole.AddOrUpdate(userRole);

                ItRole.Users.Add(userRole);

                context.SaveChanges();

            
           

    
        }
    }
}
