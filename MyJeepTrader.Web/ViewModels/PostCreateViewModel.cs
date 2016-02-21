using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using System.ComponentModel.DataAnnotations;

namespace MyJeepTrader.Web.ViewModels
{
    public class PostCreateViewModel
    {
        public tPost Post { get; set; }

        [Required(ErrorMessage = "Please enter in a title.")]
        public string Title { get; set; }

        public List<tModelWithSelected> Models { get; set; }

        public List<tYear> Years { get; set; }

        [Required(ErrorMessage = "Select a model/accessory year.")]
        public int SelectedYearId { get; set; }

        public List<tPostType> PostTypes { get; set; }

        public int SelectedPostTypeId { get; set; }

        public bool IsJeep { get; set; }

        public bool IsAccessory { get; set; }

        public HttpPostedFileBase Image { get; set; }

        public int SelectedStateId { get; set; }

        public List<tState> States { get; set; }

        public int SelectedCityId { get; set; }

        public List<tCity> Cities { get; set; }
    }

    public class tModelWithSelected
    {
        public tModel TModel { get; set; }

        public bool IsSelected { get; set; }
    }
}
