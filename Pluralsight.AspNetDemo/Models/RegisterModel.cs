using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pluralsight.AspNetDemo.Models
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string AddressLine { get; set; }
        [Required]
        public string Country { get; set; }

        [Required]
        public string role { get; set; }
    }
}