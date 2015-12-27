using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;

namespace MyJeepTrader.Web.Controllers
{
    public class BaseController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public BaseController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        
    }
}