using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MyJeepTrader.Data;
using MyJeepTrader.Data.Models;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace MyJeepTrader.Web.WebServices
{
    /// <summary>
    /// Summary description for UserInformationWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class UserInformationWebService : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public UserInformation GetUserInformation(string userId)
        {
            Service service = new Service();

            var id = Convert.ToString(userId);

            var userInfo = service.GetUserInformation(id);

            return userInfo;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<AutoCompleteUsers> GetMentionUserNames()
        {
            Service service = new Service();

            var autocomplete = service.GetAutoCompleteUsers();

            return autocomplete;
        }

    }
}
