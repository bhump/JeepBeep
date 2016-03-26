using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using MyJeepTrader.Data;
using Microsoft.AspNet.Identity;

namespace MyJeepTrader.Web.WebServices
{
    /// <summary>
    /// Summary description for NotificationWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class NotificationWebService : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int GetNotificationCount()
        {
            Service service = new Service();

            var count = service.GetNotificationCount(User.Identity.GetUserId());

            return count;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateNotification(int notificationId)
        {
            Service service = new Service();

            var notification = service.UpdateNotifcation(notificationId);

            return notification;
        }
    }
}
