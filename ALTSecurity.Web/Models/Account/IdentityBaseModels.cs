using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ALTSecurity.Web.Models.Account
{
    public class LoginModel
    {
        [Required(ErrorMessageResourceName = nameof(Resources.Global.required), ErrorMessageResourceType = typeof(Resources.Global))]
        [Display()]
        [EmailAddress(ErrorMessageResourceName = nameof(Resources.Global.valueFormat), ErrorMessageResourceType = typeof(Resources.Global))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = nameof(Resources.Global.required), ErrorMessageResourceType = typeof(Resources.Global))]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessageResourceName = nameof(Resources.Global.required), ErrorMessageResourceType = typeof(Resources.Global))]
        [MaxLength(255, ErrorMessageResourceName = nameof(Resources.Global.required), ErrorMessageResourceType = typeof(Resources.Global))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = nameof(Resources.Global.required), ErrorMessageResourceType = typeof(Resources.Global))]
        [EmailAddress(ErrorMessageResourceName = nameof(Resources.Global.valueFormat), ErrorMessageResourceType = typeof(Resources.Global))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = nameof(Resources.Global.required), ErrorMessageResourceType = typeof(Resources.Global))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = nameof(Resources.Global.required), ErrorMessageResourceType = typeof(Resources.Global))]
        [Compare(nameof(Password), ErrorMessageResourceName = nameof(Resources.Global.valueFormat), ErrorMessageResourceType = typeof(Resources.Global))]
        public string ConfirmPassword { get; set; }
    }
}