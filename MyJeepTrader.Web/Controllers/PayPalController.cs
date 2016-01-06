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
        public ActionResult Index()
        {
            PayPalIndexViewModel model = new PayPalIndexViewModel();

            var PayPalService = new PayPalService();
            var gateway = PayPalService.PayPalGateway();
            var clientToken = gateway.ClientToken.generate();
            model.ClientToken = clientToken;
            TempData["ClientToken"] = clientToken;
            return View(model);
        }

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
            model.ClientToken = TempData["ClientToken"].ToString();

            // Use payment method nonce here

            //commented out for testing.
            //var transRequest = new TransactionRequest
            //{
            //    Amount = 5.99M,
            //    PaymentMethodNonce = "fake-valid-nonce"
            //    //CreditCard = new TransactionCreditCardRequest
            //    //{
            //    //    Number = collection["number"],
            //    //    CVV = collection["cvv"],
            //    //    ExpirationMonth = collection["month"],
            //    //    ExpirationYear = collection["year"]
            //    //}
            //};
            //Result<Transaction> transResult = gateway.Transaction.Sale(transRequest);

            if(monthly == "on")
            {
                var subRequest = new SubscriptionRequest
                {
                    //PaymentMethodToken = model.ClientToken,
                    PaymentMethodNonce = "fake-valid-visa-nonce",
                    PlanId = "rphr",
                    Descriptor = new DescriptorRequest
                    {
                        Name = userInfo.FirstName + " " + userInfo.LastName
                    }
                };
                Result<Subscription> subResult = gateway.Subscription.Create(subRequest);

                if (subResult.IsSuccess())
                {
                    Subscription transaction = subResult.Target;
                    ViewData["TransactionId"] = transaction.Id;
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
                    //PaymentMethodToken = model.ClientToken,
                    PaymentMethodNonce = "fake-valid-visa-nonce",
                    PlanId = "7rjw",
                    Descriptor = new DescriptorRequest
                    {
                        Name = userInfo.FirstName + " " + userInfo.LastName
                    }
                };

                Result<Subscription> subResult = gateway.Subscription.Create(subRequest);

                if (subResult.IsSuccess())
                {
                    Subscription transaction = subResult.Target;
                    ViewData["TransactionId"] = transaction.Id;
                }
                else
                {
                    ViewData["Message"] = subResult.Message;
                }
            }

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
