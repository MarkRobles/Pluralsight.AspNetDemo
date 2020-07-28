using Microsoft.AspNet.Identity.EntityFramework;
using Pluralsight.AspNetDemo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Web;

namespace Pluralsight.AspNetDemo.DAL
{
    public class ExtendedUserDbContext: IdentityDbContext<ExtendedUser>
    {


        public DbSet<Address> Addresses { get; set; }

        public ExtendedUserDbContext(string connectionString) : base(connectionString) { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var address = modelBuilder.Entity<Address>();
            address.ToTable("AspNetUsersAddresses");
            address.HasKey(x => x.Id);


            var user = modelBuilder.Entity<ExtendedUser>();
            user.Property(x => x.FullName).IsRequired().HasMaxLength(256).HasColumnAnnotation("Index", new IndexAnnotation(
                new IndexAttribute("FullNameIndex")));

            user.HasMany(x => x.Addresses).WithRequired().HasForeignKey(x => x.UserId);
        }
    }
}