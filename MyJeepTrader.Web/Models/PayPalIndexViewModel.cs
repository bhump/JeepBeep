using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyJeepTrader.Web.Models
{
    public class PayPalIndexViewModel
    {
        public String ClientToken { get; set; }

        public string Nonce { get; set; }
    }
}