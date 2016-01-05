using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using Braintree;
using MyJeepTrader.Web.Models;

namespace MyJeepTrader.Web.Controllers
{
    public class PayPalController : Controller
    {
        // GET: PayPal
        public ActionResult Index()
        {
            PayPalIndexViewModel model = new PayPalIndexViewModel();

            var PayPalService = new PayPalService();
            var gateway = PayPalService.PayPalGateway();
            var clientToken = gateway.ClientToken.generate();
            model.ClientToken = gateway.ClientToken.generate();
            return View(model);
        }

        public ActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(FormCollection collection)
        {
            var PayPalService = new PayPalService();
            var gateway = PayPalService.PayPalGateway();

            string nonce = collection["fake-valid-nonce"];
            // Use payment method nonce here

            var request = new TransactionRequest
            {
                Amount = 5.99M,
                PaymentMethodNonce = "fake-valid-nonce"
                //CreditCard = new TransactionCreditCardRequest
                //{
                //    Number = collection["number"],
                //    CVV = collection["cvv"],
                //    ExpirationMonth = collection["month"],
                //    ExpirationYear = collection["year"]
                //}
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;
                ViewData["TransactionId"] = transaction.Id;
            }
            else
            {
                ViewData["Message"] = result.Message;
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
                //context.Response.Write(clientToken);
            }

            public bool IsReusable
            {
                get { return true; }
            }
        }
    }
}
