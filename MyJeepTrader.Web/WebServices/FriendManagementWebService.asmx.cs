using MyJeepTrader.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace MyJeepTrader.Web.WebServices
{
    /// <summary>
    /// Summary description for FriendManagementWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    [ScriptService]
    public class FriendManagementWebService : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AcceptFriend(string friendsListId)
        {
            Service service = new Service();

            var acceptFriend = service.AcceptFriend(friendsListId);

            return acceptFriend;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RejectFriend(string friendsListId)
        {
            Service service = new Service();

            var rejectFriend = service.RejectFriend(friendsListId);

            return rejectFriend;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string BlockFriend(string friendsListId, string blocked)
        {
            Service service = new Service();

            var blockFriend = service.BlockFriend(friendsListId, blocked);

            return blockFriend;
        }
    }
}
