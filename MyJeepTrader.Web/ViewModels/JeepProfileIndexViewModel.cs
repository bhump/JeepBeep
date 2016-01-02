using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyJeepTrader.Web.ViewModels
{
    public class JeepProfileIndexViewModel
    {
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

        [Display(Name = "Make Primary?")]
        public bool PrimaryJeep { get; set; }

        [Display(Name = "Description")]
        public string JeepDescription { get; set; }

        [Display(Name = "Jeep Image")]
        public byte[] Image { get; set; }
        #endregion
    }
}