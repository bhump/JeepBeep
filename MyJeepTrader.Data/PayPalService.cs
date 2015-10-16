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
    }
}
