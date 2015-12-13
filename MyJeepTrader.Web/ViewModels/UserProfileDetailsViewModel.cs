using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using System.Web;

namespace MyJeepTrader.Web.ViewModels
{
    public class UserProfileDetailsViewModel
    {
        public int UserProfileId { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Birth date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Avatar")]
        public byte[] Avatar { get; set; }

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
    }
}