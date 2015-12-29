using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;

namespace MyJeepTrader.Web.ViewModels
{
    public class PostCreateViewModel
    {
        public tPost Post { get; set; }

        public List<tModelWithSelected> Models { get; set; }

        public List<tYear> Years { get; set; }

        /// <summary>
        /// Gets or sets the selected year. This is a single year object that will be selected from the dropdownlist of years.
        /// </summary>
        /// <value>
        /// The selected year.
        /// </value>
        public int SelectedYearId { get; set; }

        public List<tPostType> PostTypes { get; set; }
        public int SelectedPostTypeId { get; set; }
        public bool IsJeep { get; set; }
        public bool IsAccessory { get; set; }
        //public byte[] Image { get; set; }

        public HttpPostedFileBase Image { get; set; }
    }

    public class tModelWithSelected
    {
        public tModel TModel { get; set; }
        public bool IsSelected { get; set; }

    }
}
