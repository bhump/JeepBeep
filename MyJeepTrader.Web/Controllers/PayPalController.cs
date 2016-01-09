using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using Braintree;
using MyJeepTrader.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace MyJeepTrader.Web.Controllers
{
    public class PayPalController : Controller
    {
        private ApplicationUserManager _userManager;

        public PayPalController()
        {

        }

        public PayPalController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
        // GET: PayPal
        [Authorize]
        public ActionResult Index()
        {
            PayPalIndexViewModel model = new PayPalIndexViewModel();
            Service service = new Service();
            var user = UserManager.FindByName(User.Identity.Name);
            var customerId = service.GetCustomer(user.Id);
            var PayPalService = new PayPalService();
            var gateway = PayPalService.PayPalGateway();
            var clientToken = gateway.ClientToken.generate(
                new ClientTokenRequest
                {
                    CustomerId = customerId
                }
                );
            model.ClientToken = clientToken;
            TempData["ClientToken"] = clientToken;
            return View(model);
        }

        [Authorize]
        public ActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(FormCollection collection)
        {
            Service service = new Service();
            PayPalIndexViewModel model = new PayPalIndexViewModel();
            var PayPalService = new PayPalService();

            var gateway = PayPalService.PayPalGateway();
            var user = UserManager.FindByName(User.Identity.Name);
            var userInfo = service.GetProfileInfo(user.Id);
            var monthly = collection["Monthly"];
            var annual = collection["Annual"];
            string nonce = collection["payment_method_nonce"];

            // Use payment method nonce here
            if (monthly == "on")
            {
                var subRequest = new SubscriptionRequest
                {
                    PaymentMethodNonce = nonce,
                    PlanId = ConstantStrings.monthlyPlanId
                };
                Result<Subscription> subResult = gateway.Subscription.Create(subRequest);

                if (subResult.IsSuccess())
                {
                    Subscription transaction = subResult.Target;
                    ViewData["TransactionId"] = transaction.Id;
                    service.UpdateMembership(User.Identity.Name, ConstantStrings.monthlySubscription, DateTime.Now);
                }
                else
                {
                    ViewData["Message"] = subResult.Message;
                }
            }
            else if (annual == "on")
            {
                var subRequest = new SubscriptionRequest
                {
                    PaymentMethodNonce = nonce,
                    PlanId = ConstantStrings.annualPlanId
                };

                Result<Subscription> subResult = gateway.Subscription.Create(subRequest);

                if (subResult.IsSuccess())
                {
                    Subscription transaction = subResult.Target;
                    ViewData["TransactionId"] = transaction.Id;
                    service.UpdateMembership(User.Identity.Name, ConstantStrings.annualSubscription, DateTime.Now);
                }
                else
                {
                    ViewData["Message"] = subResult.Message;
                }
            }

            UserManager.RemoveFromRole(user.Id, "Basic");
            UserManager.AddToRole(user.Id, "Premium");

            return View();
        }

        public class ClientTokenHandler : IHttpHandler
        {
            public void ProcessRequest(HttpContext context)
            {
                var PayPalService = new PayPalService();
                var gateway = PayPalService.PayPalGateway();
                var clientToken = gateway.ClientToken.generate();
            }

            public bool IsReusable
            {
                get { return true; }
            }
        }
    }
}
