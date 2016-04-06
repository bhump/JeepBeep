using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data
{
    public class PayPalService
    {
        public BraintreeGateway PayPalGateway()
        {
            var gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = "49d6wj6x7fbjzzm5",
                PublicKey = "fzwyjc36mk8tsmfy",
                PrivateKey = "6457701accfac3929af1290750fee1c2"
            };

            return gateway;
        }

        public Result<Subscription> PayPalSubscription(string nonce, string planId)
        {
            var request = new SubscriptionRequest
            {
                PaymentMethodNonce = nonce,
                PlanId = planId
            };

            Result<Subscription> result = PayPalGateway().Subscription.Create(request);

            return result;
        }

        public void PayPalCreateCustomer(string email, string userId, string userName, DateTime startDate, DateTime expireDate)
        {
            var request = new CustomerRequest
            {
                Email = email
            };

            Result<Customer> result = PayPalGateway().Customer.Create(request);

            bool success = result.IsSuccess();

            if(success)
            {
                Service service = new Service();
                service.UpdateMembership(result.Target.Id, userId);

                var memberInfo = service.GetMembership(userName);

                service.CreateSubscription(memberInfo.MembershipId, startDate, startDate.AddYears(100), ConstantStrings.freeSubscription, null);
            }
        }

        public void PayPalCancelSubscription(string subscriptionId)
        {
            var result = PayPalGateway().Subscription.Cancel(subscriptionId);
        }
    }
}
