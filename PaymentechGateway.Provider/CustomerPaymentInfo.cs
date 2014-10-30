using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentechGateway.Provider
{
    public class CustomerPaymentInfo
    {
        public string EmailAddress { get; set; }
        public CardInfo CardInfo { get; set; }
        public BillingAddressInfo BillingAddressInfo { get; set; }
    }
}
