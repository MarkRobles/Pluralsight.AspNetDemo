using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pluralsight.AspNetDemo.Models
{
    public class Address
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }

        public string AddressLine { get; set; }
        public string Country { get; set; }
    }
}