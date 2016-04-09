using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyJeepTrader.Data;

namespace MyJeepTrader.Web.ViewModels
{
    public class StatusCreateViewModel
    {
        public tStatusUpdate statusUpdate { get; set; }

        public string status { get; set; }

        public DateTime dateCreated { get; set; }

        public bool privateBool { get; set; }

        public int likeCount { get; set; }

        public HttpPostedFileBase Image { get; set; }

        public tMention mention { get; set; }
    }
}