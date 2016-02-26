using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyJeepTrader.Data;
using MyJeepTrader.Data.Models;

namespace MyJeepTrader.Web.ViewModels
{
    public class LiveStreamViewModel
    {
        public IEnumerable<LiveStream> LiveStreams { get; set; }

        public IEnumerable<tPost> LivePosts { get; set; }
    }
}