using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyJeepTrader.Data;
using MyJeepTrader.Data.Models;

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
        [Display(Name = "Current")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        #endregion

        #region user profile
        [Display(Name = "UserProfileId")]
        public int UserProfileId { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Birth date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<DateTime> BirthDate { get; set; }

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

        [Display(Name = "Username")]
        public string UserName { get; set; }
        #endregion

        #region jeep profile
        public int JeepProfileId { get; set; }

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

        public List<MailMessages> SentMessages { get; set; }

        public List<MailMessages> Inbox { get; set; }

        public int MessageId { get; set; }

        #endregion

        #region membership and subscription
        [Display(Name = "MembershipId")]
        public int MembershipId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}",
               ApplyFormatInEditMode = true)]
        [Display(Name = "Member Since")]
        public DateTime? MemberSince { get; set; }

        [Display(Name = "Expired")]
        public bool? Expired { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}",
               ApplyFormatInEditMode = true)]
        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Auto Renew")]
        public bool? AutoRenew { get; set; }

        [Display(Name = "Renewed")]
        public bool? Renewed { get; set; }

        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "AspNetUser")]
        public AspNetUser AspNetUser { get; set; }

        public tSubscription tSubscription { get; set; }

        public string SubscriptionType { get; set; }
        #endregion

        #region Admin
        public IEnumerable<AspNetUser> Users { get; set; }

        [Display(Name = "New Password:")]
        public string ResetPassword { get; set; }
        #endregion
    }
}