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
                MerchantId = "hpjd63fycvh7j38d",
                PublicKey = "59yht5x4w7rn4pj7",
                PrivateKey = "6d39af34433ba58f94be9153a77eb427"
            };

            return gateway;
        }

        //not currently used.
        public string PayPalSubscription(string planId, string clientToken, string name)
        {
            var request = new SubscriptionRequest
            {
                PaymentMethodToken = clientToken,
                PlanId = planId,
                Descriptor = new DescriptorRequest
                {
                    Name = name
                }
            };

            Result<Subscription> result = PayPalGateway().Subscription.Create(request);

            return result.Message;
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
