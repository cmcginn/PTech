using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentechGateway.Provider
{
    public class RefundPaymentRequest
    {
        public string CustomerRefNum { get; set; }

        public string TransactionRefNum { get; set; }

        public string AuthorizationCode { get; set; }

        public string GatewayOrderId { get; set; }

        public double OrderTotal { get; set; }

        public double RefundTotal { get; set; }

        public double OrderTax { get; set; }
    }
}
