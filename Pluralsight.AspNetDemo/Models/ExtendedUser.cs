using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.BuilderProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pluralsight.AspNetDemo.Models
{
    public class ExtendedUser : IdentityUser
    {

        public ExtendedUser() {
            Addresses = new List<Address>();
        }
        public string FullName { get; set; }
        public virtual ICollection<Address> Addresses  { get;  private set; }
    }
}