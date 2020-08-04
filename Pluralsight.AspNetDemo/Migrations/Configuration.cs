namespace Pluralsight.AspNetDemo.Migrations
{
    using Antlr.Runtime.Tree;
    using Microsoft.AspNet.Identity.EntityFramework;
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

        protected override void Seed(Pluralsight.AspNetDemo.DAL.ExtendedUserDbContext context)
        {
            var ItRole = new ExtendedRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "IT",
                Details = "Super admin"
            };

            context.SaveChanges();
            var UserAdmin = new ExtendedUser
            {
                UserName = "marcos92robles@gmail.com",
                Email = "marcos92robles@gmail.com",
                FullName = "Marcos Ivan Robles Hernandez",
                PhoneNumber = "+523316901969"

            };



            context.SaveChanges();

            var userRole = new IdentityUserRole
            {
                RoleId = ItRole.Id,
                UserId = UserAdmin.Id
            };


            ItRole.Users.Add(userRole);

            context.SaveChanges();

            context.Roles.AddOrUpdate();
        }
    }
}
