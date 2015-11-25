using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyJeepTrader.Web.ViewModels
{
    public class DashboardIndexViewModel
    {
        #region reset email
        public string Email { get; set; }
        #endregion

        #region reset password
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        #endregion

        #region update profile
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Birth date")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Avatar")]
        public byte Avatar { get; set; }

        [Display(Name = "About you")]
        public string Description { get; set; }
        
        [Display(Name = "Facebook url")]
        public string Facebook { get; set; }
        
        [Display(Name = "Twitter url")]
        public string Twitter { get; set; }
        
        [Display(Name = "Ello url")]
        public string Ello { get; set; }
        
        [Display(Name = "Google+ url")]
        public string GooglePlus { get; set; }

        [Display(Name = "Your website")]
        public string Website { get; set; }
        #endregion
    }
}