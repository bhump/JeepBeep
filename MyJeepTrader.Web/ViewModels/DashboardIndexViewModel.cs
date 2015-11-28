﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyJeepTrader.Data;

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
        #endregion

        #region jeep profile
        [Display(Name = "Manufactuer")]
        public string Manufactuer { get; set; }

        [Display(Name = "Make")]
        public string Make { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Year")]
        public short Year { get; set; }

        [Display(Name = "Primary Jeep")]
        public bool PrimaryJeep { get; set; }

        [Display(Name = "Description")]
        public string JeepDescription { get; set; }

        [Display(Name = "Jeep Image")]
        public byte[] Image { get; set; }
        #endregion

        #region mailbox
        [Display(Name = "To")]
        public string To { get; set; }

        [Display(Name = "From")]
        public string From { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Date Sent")]
        public DateTime DateSent { get; set; }

        [Display(Name = "Read On")]
        public DateTime DateRead { get; set; }

        public List<MyJeepTrader.Data.Service.SentMessages> SentMessages { get; set; }

        //public IEnumerable<tMessageControl> MessageControl { get; set; }

        #endregion
    }
}