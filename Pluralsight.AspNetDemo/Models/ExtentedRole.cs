using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pluralsight.AspNetDemo.Models
{
    public class ExtentedRole:IdentityRole
    {
        public string Details { get; set; }
    }
}