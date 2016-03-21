using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MyJeepTrader.Data;
using MyJeepTrader.Data.Models;
using System.Web.Script.Services;

namespace MyJeepTrader.Web.WebServices
{
    /// <summary>
    /// Summary description for CommentsWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class CommentsWebService : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Comments> GetCommentsByStatusId(int statusId)
        {
            Service service = new Service();
            var comments = service.GetCommentsForStatus(statusId);

            return comments;
        }
    }
}
