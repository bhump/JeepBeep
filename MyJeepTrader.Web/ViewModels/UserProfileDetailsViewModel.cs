using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using MyJeepTrader.Data.Models;
using System.Web;

namespace MyJeepTrader.Web.ViewModels
{
    public class UserProfileDetailsViewModel
    {
        public int UserProfileId { get; set; }

        #region User Profile
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

        [Display(Name = "Username")]
        public string UserName { get; set; }
        #endregion

        #region primary jeep profile
        public int JeepProfileId { get; set; }

        [Display(Name = "Manufactuer")]
        public string Manufactuer { get; set; }

        [Display(Name = "Make")]
        public string Make { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Year")]
        public short ?Year { get; set; }

        [Display(Name = "Primary Jeep")]
        public bool PrimaryJeep { get; set; }

        [Display(Name = "Description")]
        public string JeepDescription { get; set; }

        [Display(Name = "Jeep Image")]
        public byte[] Image { get; set; }

        [Display(Name = "Profile Views")]
        public long ?ViewCount { get; set; }
        #endregion

        #region posts
        public List<UsersPosts> UserPosts { get; set; }

        public UsersPosts RecentPost { get; set; }
        #endregion
    }
}