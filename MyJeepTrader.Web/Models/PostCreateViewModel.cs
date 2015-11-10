using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyJeepTrader.Data;

namespace MyJeepTrader.Web.Models
{
    public class PostCreateViewModel
    {
        public tPost Post { get; set; }

        public IList<tModel> Models { get; set; }
 

    }
}
