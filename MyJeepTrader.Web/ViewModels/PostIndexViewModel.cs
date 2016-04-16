using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyJeepTrader.Data;
using MyJeepTrader.Data.Models;

namespace MyJeepTrader.Web.ViewModels
{
    public class PostIndexViewModel
    {
        public IEnumerable<tPost> Posts { get; set; }

        public IEnumerable<tPost> SearchedPosts { get; set; } 
    }
}
