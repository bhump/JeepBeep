using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MyJeepTrader.Data;
using MyJeepTrader.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace MyJeepTrader.Web.WebServices
{
    /// <summary>
    /// Summary description for SettingsWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SettingsWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string UpdateSettings(string posts, string status)
        {
            var userId = User.Identity.GetUserId();

            Service service = new Service();

            var response = service.UpdateSettings(userId, posts, status);

            return response;
        }

        [WebMethod]
        public string UpdatePrivacy(string privateOnly)
        {
            var userId = User.Identity.GetUserId();

            Service service = new Service();

            var response = service.UpdateSettings(userId, privateOnly);

            return response;
        }
    }
}
