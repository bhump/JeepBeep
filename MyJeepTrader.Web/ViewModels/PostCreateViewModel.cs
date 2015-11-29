using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyJeepTrader.Data;

namespace MyJeepTrader.Web.ViewModels
{
    public class PostCreateViewModel
    {
        public tPost Post { get; set; }

        public IList<tModel> Models { get; set; }

        public List<tYear> Years { get; set; }

        /// <summary>
        /// Gets or sets the selected year. This is a single year object that will be selected from the dropdownlist of years.
        /// </summary>
        /// <value>
        /// The selected year.
        /// </value>
        public tYear SelectedYear { get; set; }

        public List<tPostType> PostTypes { get; set; }

        public bool IsJeep { get; set; }
        public bool IsAccessory { get; set; }

    }
}
