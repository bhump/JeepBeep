using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyJeepTrader.Web.Controllers
{
    public class InstantChatController : Controller
    {
        // GET: InstantChat
        public ActionResult ChatPartial()
        {
            return View("~/Views/Shared/_ChatPartial.cshtml");
        }
    }
}