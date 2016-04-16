using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyJeepTrader.Web.ViewModels
{
    public class FeedbackViewModel
    {
        [Required]
        [Range(1, 5)]
        [Display(Name = "Scale 1-5 of Concept: ")]
        [StringLength(100, ErrorMessage = "Please add an overall rating of the concept of MyJeepTrader.com")]
        public int Scale { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Overall site functionality: ")]
        [StringLength(100, ErrorMessage = "Please add an overall rating of the site's functionality.")]
        public int Overall { get; set; }
        
        [Required]
        [Display(Name = "Explanation of ratings: ")]
        [StringLength(100, ErrorMessage = "Please add an explanation for your ratings.")]
        public string Description { get; set; }
        
        public string Id { get; set; }

    }
}